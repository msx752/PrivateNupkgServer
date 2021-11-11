namespace privatenupkgserver.Models.Nuget;

public class NugetQueryServiceModel
{
    public string? q { get; set; }

    public int skip { get; set; }

    public int take { get; set; }

    public bool prerelease { get; set; }

    public string[]? supportedFramework { get; set; }

    public string? semVerLevel { get; set; }

    [XmlIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public NuGetVersionString NuGetVersion { get => (NuGetVersionString)semVerLevel; }
}
