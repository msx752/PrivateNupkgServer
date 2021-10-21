using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using privatenupkgserver.Enums;
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

namespace privatenupkgserver.Services.AzureBlobStorage
{
    public class AzureBlobStorageService : StorageService, IAzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ICacheService cacheService;
        private BlobContainerClient blobContainerClient;

        public BlobContainerClient BlobContainerClient
        {
            get
            {
                if (blobContainerClient == null)
                    blobContainerClient = _blobServiceClient.GetBlobContainerClient(nugetServerOption.PackageDirectory);
                return blobContainerClient;
            }
        }

        public AzureBlobStorageService(
            IOptions<NugetServerOption> nugetServerOption,
            IOptions<NugetServerStorageOption> nugetServerStorageOption,
            ICacheService cacheService) : base(nugetServerOption, nugetServerStorageOption, cacheService)
        {
            if (!string.IsNullOrWhiteSpace(nugetServerStorageOption.Value.AzureStorageConnectionString) && nugetServerStorageOption.Value.SelectedType == NugetStorageType.AzureBlobStorage)
            {
                _blobServiceClient = new BlobServiceClient(nugetServerStorageOption.Value.AzureStorageConnectionString);
                foreach (var item in _blobServiceClient.GetBlobContainers(BlobContainerTraits.None))
                    if (item.Name.Equals(nugetServerOption.Value.PackageDirectory, StringComparison.InvariantCultureIgnoreCase))
                        return;

                _blobServiceClient.CreateBlobContainer(nugetServerOption.Value.PackageDirectory, PublicAccessType.None);
            }
            this.cacheService = cacheService;
        }

        public async override Task AddNupkgAsync(string nugetId, string nugetVersion, Stream package)
        {
            var fileName = CombineToFilePath(nugetId, nugetVersion);
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            await BlobContainerClient.UploadBlobAsync(fileName, package);
        }

        public override string GetPackageRootFullPath()
        {
            return "";
        }

        public override RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInput)
        {
            return base.GetRegistrationIndex(registrationInput);
        }

        private Stream GetNuspecStream(string filePath)
        {
            MemoryStream stream = new MemoryStream();
            var selectedBlob = BlobContainerClient.GetBlobClient(filePath);
            selectedBlob.DownloadTo(stream);
            return stream;
        }

        public override Stream GetNupkg(string packageName, string packageVersion)
        {
            var filePath = GetNuspecName(packageName, packageVersion);
            Stream stream = GetNupkgStream(filePath);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public override Nuspec GetNuspec(string nugetId, string nugetVersion)
        {
            return ListNuspecPackage().Where(x => x.Equals(CombineToFilePath(nugetId, nugetVersion))).FirstOrDefault();
        }

        private Stream GetNupkgStream(string filePath)
        {
            var selectedBlob = BlobContainerClient.GetBlobClient(filePath);
            if (!selectedBlob.Exists())
                return null;
            MemoryStream stream = new MemoryStream();
            selectedBlob.DownloadTo(stream);
            return stream;
        }

        public override IEnumerable<Nuspec> ListNuspecPackage()
        {
            var blobsPaged = BlobContainerClient.GetBlobs();
            foreach (var item in blobsPaged)
            {
                Stream stream = GetNupkgStream(item.Name);
                Nuspec nspc = Zip.ReadNuspecFromPackageAsync(stream).Result;
                nspc.PublishTime = item.Properties.CreatedOn;
                yield return nspc;
            }
        }

        public override SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl)
        {
            return base.SearchQuery(nugetQueryModel, baseUrl);
        }

        public override bool ContainsNuspec(string nugetId, string nugetVersion)
        {
            return GetNuspecName(nugetId, nugetVersion) != null;
        }

        public override string CombineToFilePath(string id, string version)
        {
            return base.CombineToFilePath(id, version).Replace("\\", "/");
        }

        public override string GetNuspecName(string nugetId, string nugetVersion)
        {
            return ListNuspecName().Where(f => f.Equals(CombineToFilePath(nugetId, nugetVersion))).FirstOrDefault();
        }

        public override IEnumerable<string> ListNuspecName()
        {
            var blobsPaged = BlobContainerClient.GetBlobs();
            foreach (var item in blobsPaged)
                yield return item.Name;
        }
    }
}