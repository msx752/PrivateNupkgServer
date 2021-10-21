using Microsoft.Extensions.FileProviders.Physical;
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

namespace privatenupkgserver.Services.LocalDisk
{
    public class LocalDiskStorageService : StorageService, ILocalDiskStorageService
    {
        public LocalDiskStorageService(
            IOptions<NugetServerOption> nugetServerOption,
            IOptions<NugetServerStorageOption> nugetServerStorageOption,
            ICacheService cacheService) : base(nugetServerOption, nugetServerStorageOption, cacheService)
        {
        }

        public override void DeleteNupkg(string filePath, string nugetId, string nugetVersion)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException("Package not found.", filePath);
            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                throw;
            }
            base.DeleteNupkg(null, nugetId, nugetVersion);
        }

        public override Stream GetNupkg(string packageName, string packageVersion)
        {
            var filePath = GetNuspecName(packageName, packageVersion);
            var fileInfo = new PhysicalFileInfo(new FileInfo(filePath));
            var stream = fileInfo.CreateReadStream();
            return stream;
        }

        public override SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl)
        {
            return base.SearchQuery(nugetQueryModel, baseUrl);
        }

        public override bool ContainsNuspec(string nugetId, string nugetVersion)
        {
            return GetNuspecName(nugetId, nugetVersion) != null;
        }

        public override string GetNuspecName(string nugetId, string nugetVersion)
        {
            return ListNuspecName().Where(x => x.Equals(CombineToFilePath(nugetId, nugetVersion))).FirstOrDefault();
        }

        public override Nuspec GetNuspec(string nugetId, string nugetVersion)
        {
            return ListNuspecPackage().Where(x => x.Equals(CombineToFilePath(nugetId, nugetVersion))).FirstOrDefault();
        }

        public override IEnumerable<string> ListNuspecName()
        {
            if (!Directory.Exists(GetPackageRootFullPath()))
                Directory.CreateDirectory(GetPackageRootFullPath());

            return Directory.EnumerateFiles(GetPackageRootFullPath(), "*.nupkg", SearchOption.AllDirectories);
        }

        private static bool True(string s) => true;

        public override IEnumerable<Nuspec> ListNuspecPackage()
        {
            foreach (var filePath in ListNuspecName())
            {
                Nuspec nuspec = Zip.ReadNuspec(filePath);
                if (nuspec.Metadata == null)
                    continue;
                nuspec.FilePath = filePath;
                nuspec.PublishTime = new DateTimeOffset(new FileInfo(nuspec.FilePath).LastWriteTimeUtc);
                yield return nuspec;
            }
        }

        public override async Task AddNupkgAsync(string nugetId, string nugetVersion, Stream package)
        {
            var fileName = CombineToFilePath(nugetId, nugetVersion);
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
                throw new InvalidOperationException("Package file exists.");

            try
            {
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                using (var fs = fileInfo.OpenWrite())
                {
                    if (!fs.CanWrite)
                        throw new InvalidOperationException("Package file cannot write.");
                    try
                    {
                        await package.CopyToAsync(fs, 4096);
                    }
                    catch (Exception e1)
                    {
                        fileInfo.Delete();
                        throw;
                    }
                }
            }
            catch (Exception e2)
            {
                throw;
            }
            await base.AddNupkgAsync(nugetId, nugetVersion, package);
        }

        public override string GetPackageRootFullPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(nugetServerOption?.PackageDirectory))
                return baseDir;

            return Path.Combine(baseDir, nugetServerOption.PackageDirectory);
        }

        public override RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInput)
        {
            return base.GetRegistrationIndex(registrationInput);
        }
    }
}