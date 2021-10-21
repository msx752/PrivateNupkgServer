using privatenupkgserver.Enums;

namespace privatenupkgserver.Options
{
    public class NugetServerStorageOption
    {
        public NugetServerStorageOption()
        {
            SelectedType = NugetStorageType.LocalDisk;
        }

        public NugetStorageType SelectedType { get; set; }
        public string AzureStorageConnectionString { get; set; }
    }
}