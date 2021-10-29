using Microsoft.Extensions.Options;
using NuGet.Versioning;
using privatenupkgserver.Enums;
using privatenupkgserver.Extensions;
using privatenupkgserver.Models.Nuget;
using privatenupkgserver.Models.Nuspec;
using privatenupkgserver.Models.Registration;
using privatenupkgserver.Models.SearchQuery;
using privatenupkgserver.Options;
using privatenupkgserver.Services.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace privatenupkgserver.Services
{
    public abstract class StorageService : IStorageService
    {
        public readonly NugetServerOption nugetServerOption;
        public readonly NugetServerStorageOption nugetServerStorageOption;
        private readonly ICacheService cacheService;

        public StorageService(
            IOptions<NugetServerOption> nugetServerOption,
            IOptions<NugetServerStorageOption> nugetServerStorageOption,
            ICacheService cacheService)
        {
            this.nugetServerOption = nugetServerOption.Value;
            this.nugetServerStorageOption = nugetServerStorageOption.Value;
            this.cacheService = cacheService;
        }

        public virtual void DeleteNupkg(string filePath, string nugetId, string nugetVersion)
        {
            //if (nugetServerOption.CacheEnabled)
            //    cacheService.DeleteCachedNupkg(nugetId, nugetVersion);
        }

        public abstract string GetPackageRootFullPath();

        public virtual string CombineToFilePath(string id, string version)
        {
            var lowerId = id.ToLowerInvariant();
            var lowerVersion = version.ToLowerInvariant();
            var path = Path.Combine(GetPackageRootFullPath(), lowerId, lowerVersion, $"{lowerId}.{lowerVersion}.nupkg");
            return path;
        }

        public virtual SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl)
        {
            string registrationsBaseUrl = $"{baseUrl}{nugetServerOption.GetResourceUrlPath(NugetServerResourceType.RegistrationsBaseUrl)}";

            var metadata_versions = ListNuspecPackage()
                .Where(nuspec => nuspec.Metadata != null)
                .Select(nuspec => new { nuspec.Metadata, Version = (NuGetVersionString)nuspec.Metadata.Version })
                .Where(mv => nugetQueryModel.NuGetVersion?.Major != 1 ? true : !mv.Version.IsSemVer2)
                .Where(mv => nugetQueryModel.prerelease ? true : !mv.Version.IsPrerelease)
                .OrderBy(mv => mv.Metadata.Id)
                .GroupBy(mv => mv.Metadata.Id)
                .Where(mv => string.IsNullOrWhiteSpace(nugetQueryModel.q) || mv.Key.ToLowerInvariant().Contains(nugetQueryModel.q.ToLowerInvariant()))
                .ToList();

            var takes = metadata_versions
                .Skip(nugetQueryModel.skip)
                .Take(nugetQueryModel.take)
                .ToList();

            var searchOutputModel = new SearchOutputModel
            {
                TotalHits = metadata_versions.Count
            };

            foreach (var path_metadata in takes)
            {
                var packageId = path_metadata.Key;
                var packageIdLowerInvariant = packageId.ToLowerInvariant();

                var metadatas = path_metadata.Select(x => x.Metadata).OrderBy(x => x.Version).ToList();

                var latest = metadatas.Last();
                searchOutputModel.Data.Add(new SearchResultModel
                {
                    Id = latest.Id,
                    Authors = latest.Authors?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>(0),
                    Description = latest.Description,
                    Owners = latest.Owners?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>(0),
                    ProjectUrl = latest.ProjectUrl,
                    IconUrl = latest.IconUrl,
                    Registration = $"{registrationsBaseUrl}/{packageIdLowerInvariant}/index.json",
                    Id_alias = $"{registrationsBaseUrl}/{packageIdLowerInvariant}/index.json",
                    Summary = latest.ReleaseNotes ?? latest.Description,
                    Tags = latest.Tags?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>(0),
                    Title = latest.Id,
                    Version = latest.Version,
                    LicenseUrl = latest.LicenseUrl,
                    Versions = metadatas.Select(m => new SearchResultPackageVersionModel
                    {
                        Id = $"{registrationsBaseUrl}/{packageIdLowerInvariant}/{m.Version}.json",
                        Version = m.Version
                    }).ToList(),
                });
            }

            return searchOutputModel;
        }

        public virtual Task AddNupkgAsync(string nugetId, string nugetVersion, Stream package)
        {
            //if (nugetServerOption.CacheEnabled)
            //{
            //    string cacheId = nugetId.ToLowerInvariant();
            //    string cacheVersion = nugetVersion;
            //    string filePath = CombineToFilePath(nugetId, nugetVersion);
            //    package.Seek(0, SeekOrigin.Begin);
            //    Nuspec nuspec = Zip.ReadNuspec(filePath);
            //    nuspec.FilePath = filePath;
            //    cacheService.UpdateCachedNuspec(cacheId, cacheVersion, nuspec);
            //    package.Seek(0, SeekOrigin.Begin);
            //    byte[] buff = new byte[package.Length];
            //    package.Read(buff, 0, buff.Length);
            //    cacheService.UpdateCachedNupkg(cacheId, cacheVersion, buff);
            //}
            return Task.CompletedTask;
        }

        public virtual string GetNuspecName(string nugetId, string nugetVersion)
        {
            throw new NotImplementedException();
        }

        public virtual bool ContainsNuspec(string nugetId, string nugetVersion)
        {
            throw new NotImplementedException();
        }

        public virtual Nuspec GetNuspec(string nugetId, string nugetVersion)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<string> ListNuspecName()
        {
            throw new NotImplementedException();
        }

        public abstract IEnumerable<Nuspec> ListNuspecPackage();

        public virtual RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInput)
        {
            var nuspecList = ListNuspecPackage()
                .Select(x => new { Nuspec = x, Version = (NuGetVersionString)x.Metadata.Version })
                .OrderBy(x => x.Version)
                .Select(x => x.Nuspec)
                .ToList();

            var lowest = nuspecList.First().Metadata;
            var uppest = nuspecList.Last().Metadata;
            var parent = registrationInput.BaseUrl + registrationInput.Path;
            string lowestVersion = "";
            if (SemanticVersion.TryParse(lowest.Version, out SemanticVersion semanticVersionLowest))
                lowestVersion = semanticVersionLowest.ToFullString();
            else
                lowestVersion = lowest.Version;

            string uppestVersion = "";
            if (SemanticVersion.TryParse(uppest.Version, out SemanticVersion semanticVersionuppest))
                uppestVersion = semanticVersionuppest.ToFullString();
            else
                uppestVersion = lowest.Version;

            var packageContentBase = registrationInput.BaseUrl + nugetServerOption.GetResourceUrlPath(NugetServerResourceType.PackageBaseAddress);
            var model = new RegistrationIndexOutputModel
            {
                Count = nuspecList.Count
            };

            foreach (var nuspec in nuspecList)
            {
                var publishTime = nuspec.PublishTime.Value;
                var metadata = nuspec.Metadata;
                var modelItem = new RegistrationPageOutputModel()
                {
                    Id = $"{registrationInput.BaseUrl}{registrationInput.Path}#page/{metadata.Version}/{metadata.Version}",
                    Count = nuspecList.Count,
                    Lower = lowestVersion,
                    Upper = uppestVersion,
                };
                // If present Items, MUST also present Parent,Id need NOT be used.
                modelItem.Parent = parent;
                modelItem.Items = new List<RegistrationLeafOutputModel>()
                {
                    new RegistrationLeafOutputModel
                    {
                         Id =  $"{registrationInput.BaseUrl + registrationInput.PathBase}/{metadata.Version}.json",
                         PackageContent =  $"{packageContentBase}/{metadata.Id.ToLowerInvariant()}/{metadata.Version.ToLowerInvariant()}/{metadata.Id.ToLowerInvariant()}.{metadata.Version.ToLowerInvariant()}.nupkg",
                         Registration = registrationInput.BaseUrl + registrationInput.Path,
                         CatalogEntry = new RegistrationCatalogEntryOutputModel
                         {
                            Id_alias = registrationInput.BaseUrl + registrationInput.PathBase + $"/{metadata.Version}.json",
                            Id = metadata.Id,
                            Authors = metadata.Authors,
                            Description = metadata.Description,
                            ProjectUrl = metadata.ProjectUrl,
                            IconUrl = metadata.IconUrl,
                            Summary = metadata.ReleaseNotes ?? metadata.Description,
                            Tags = metadata.Tags?.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?.ToList() ?? new List<string>(0),
                            Title = metadata.Id,
                            Version = metadata.Version,
                            LicenseUrl = metadata.LicenseUrl,
                            Listed = !IsDeleted(metadata.Id,metadata.Version),
                            RequireLicenseAcceptance = metadata.RequireLicenseAcceptance,
                            PackageContent =  $"{packageContentBase}/{metadata.Id.ToLowerInvariant()}/{metadata.Version.ToLowerInvariant()}/{metadata.Id.ToLowerInvariant()}.{metadata.Version.ToLowerInvariant()}.nupkg",
                            Published = publishTime,
                            DependencyGroups = metadata.Dependencies.Select(deps=>
                                new RegistrationDependencyGroupOutputModel
                                {
                                    //Id_alias = ""
                                    TargetFramework = deps.TargetFramework,
                                    Dependencies  = deps.Dependency.Select(dep=>
                                        new RegistrationDependencyOutputModel
                                        {
                                                Id = dep.Id,
                                                //Id_Alias = "",
                                                Range = dep.Version,
                                                Registration = registrationInput.BaseUrl +
                                                    registrationInput.Path
                                        }).ToList()
                                }).ToList()
                         }
                    }
                };
                model.Items.Add(modelItem);
            }
            return model;
        }

        public virtual bool IsDeleted(string id, string version)
        {
            var unlistEnabled = nugetServerOption?.InterpretUnListEnabled == true;
            if (!unlistEnabled)
                return false;
            throw new NotImplementedException("This method must be implemented by derived class.");
        }

        public virtual Stream GetNupkg(string packageName, string packageVersion)
        {
            throw new NotImplementedException();
        }
    }
}