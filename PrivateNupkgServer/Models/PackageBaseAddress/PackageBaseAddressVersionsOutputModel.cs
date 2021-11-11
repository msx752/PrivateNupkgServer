namespace privatenupkgserver.Models.PackageBaseAddress;

public class PackageBaseAddressVersionsOutputModel
{
    public PackageBaseAddressVersionsOutputModel()
    {
        Versions = new List<string>(0);
    }

    public List<string> Versions { get; set; }
}
