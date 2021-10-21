using privatenupkgserver.Enums;
using privatenupkgserver.Options;
using System;
using System.IO;

namespace privatenupkgserver.Extensions
{
    public static class NugetServerStorageExtension
    {
        private static string GetPackageRootFullPath(NugetServerOption nugetServerOption, NugetServerStorageOption nugetServerStorageOption)
        {
            if (nugetServerStorageOption.SelectedType == NugetStorageType.LocalDisk)
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (string.IsNullOrWhiteSpace(nugetServerOption?.PackageDirectory))
                    return baseDir;

                return Path.Combine(baseDir, nugetServerOption.PackageDirectory);
            }
            else if (nugetServerStorageOption.SelectedType == NugetStorageType.AzureBlobStorage)
                return "";
            else
                throw new NotImplementedException();
        }

        private static string CombineToFilePath(NugetServerOption nugetServerOption, NugetServerStorageOption nugetServerStorageOption, string id, string version)
        {
            var lowerId = id.ToLowerInvariant();
            var lowerVersion = version.ToLowerInvariant();
            return Path.Combine(GetPackageRootFullPath(nugetServerOption, nugetServerStorageOption), lowerId, lowerVersion, $"{lowerId}.{lowerVersion}.nupkg");
        }
    }
}