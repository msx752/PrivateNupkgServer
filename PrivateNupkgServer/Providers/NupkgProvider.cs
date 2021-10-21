using Microsoft.Extensions.Options;
using privatenupkgserver.Enums;
using privatenupkgserver.Extensions;
using privatenupkgserver.Models.Nuget;
using privatenupkgserver.Models.Nuspec;
using privatenupkgserver.Models.PackageBaseAddress;
using privatenupkgserver.Models.Registration;
using privatenupkgserver.Models.SearchQuery;
using privatenupkgserver.Models.ServerIndex;
using privatenupkgserver.Options;
using privatenupkgserver.Services.AzureBlobStorage;
using privatenupkgserver.Services.Cache;
using privatenupkgserver.Services.LocalDisk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace privatenupkgserver.Providers
{
    public class NupkgProvider : INupkgProvider
    {
        private readonly NugetServerOption nugetServerOption;
        private readonly NugetServerStorageOption nugetServerStorageOption;
        private readonly ILocalDiskStorageService localDiskStorageService;
        private readonly IAzureBlobStorageService azureBlobStorageService;

        public NupkgProvider(
            IOptions<NugetServerOption> nugetServerOption,
            IOptions<NugetServerStorageOption> azureBlobStorageOption,
            ILocalDiskStorageService localDiskStorageService,
            IAzureBlobStorageService azureBlobStorageService)
        {
            this.nugetServerOption = nugetServerOption.Value;
            this.nugetServerStorageOption = azureBlobStorageOption.Value;
            this.localDiskStorageService = localDiskStorageService;
            this.azureBlobStorageService = azureBlobStorageService;
        }

        public async Task AddNupkgAsync(string nugetId, string nugetVersion, Stream nuspecStream)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                await localDiskStorageService.AddNupkgAsync(nugetId, nugetVersion, nuspecStream);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                await azureBlobStorageService.AddNupkgAsync(nugetId, nugetVersion, nuspecStream);
            else
                throw new NotImplementedException();
        }

        public void DeleteNupkg(string filePath, string nugetId, string nugetVersion)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                localDiskStorageService.DeleteNupkg(filePath, nugetId, nugetVersion);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                azureBlobStorageService.DeleteNupkg(filePath, nugetId, nugetVersion);
            else
                throw new NotImplementedException();
        }

        public Stream GetNupkg(string nugetId, string nugetVersion)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.GetNupkg(nugetId, nugetVersion);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.GetNupkg(nugetId, nugetVersion);
            else
                throw new NotImplementedException();
        }

        public PackageBaseAddressVersionsOutputModel GetNugetIdVersions(string nugetId)
        {
            PackageBaseAddressVersionsOutputModel packageBaseAddressVersionsOutputModel = new PackageBaseAddressVersionsOutputModel()
            {
                Versions = this.ListNuspec()
                            .Where(n => n.Metadata != null)
                            .Where(f => f.Metadata.Id.Equals(nugetId, StringComparison.InvariantCultureIgnoreCase))
                            .Select(f => f.Metadata.Version)
                            .OrderBy(x => (NuGetVersionString)x)
                            .ToList()
            };
            return packageBaseAddressVersionsOutputModel;
        }

        public SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.SearchQuery(nugetQueryModel, baseUrl);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.SearchQuery(nugetQueryModel, baseUrl);
            else
                throw new NotImplementedException();
        }

        public ServerIndexModel GetServerIndex(string baseUrl)
        {
            var supportedResouces = nugetServerOption?.Resources
                .Select(f => f.Key)
                .Select(item => new ServerIndexResourceModel()
                {
                    Type = item.GetText(),
                    Id = baseUrl + nugetServerOption.GetResourceUrlPath(item)
                });
            var serverIndex = new ServerIndexModel(supportedResouces)
            {
                Version = nugetServerOption.NugetApiVersion.ToFullString(),
                Context = ServerIndexContext.Default
            };
            return serverIndex;
        }

        public bool ContainsNuspec(string nugetId, string nugetVersion)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.ContainsNuspec(nugetId, nugetVersion);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.ContainsNuspec(nugetId, nugetVersion);
            else
                throw new NotImplementedException();
        }

        public string GetNuspecName(string nugetId, string nugetVersion)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.GetNuspecName(nugetId, nugetVersion);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.GetNuspecName(nugetId, nugetVersion);
            else
                throw new NotImplementedException();
        }

        public Nuspec GetNuspec(string nugetId, string nugetVersion)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.GetNuspec(nugetId, nugetVersion);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.GetNuspec(nugetId, nugetVersion);
            else
                throw new NotImplementedException();
        }

        public IEnumerable<Nuspec> ListNuspec()
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.ListNuspecPackage();
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.ListNuspecPackage();
            else
                throw new NotImplementedException();
        }

        public RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInputModel)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
                return localDiskStorageService.GetRegistrationIndex(registrationInputModel);
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return azureBlobStorageService.GetRegistrationIndex(registrationInputModel);
            else
                throw new NotImplementedException();
        }
    }
}