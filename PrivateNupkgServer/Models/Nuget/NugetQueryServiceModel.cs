namespace privatenupkgserver.Models.Nuget;

public class NugetQueryServiceModel
{
    [FromQuery]
    public string? q { get; set; }

    [FromQuery]
    public int skip { get; set; }

    [FromQuery]
    public int take { get; set; }

    [FromQuery]
    public bool prerelease { get; set; }

    [FromQuery]
    public string[]? supportedFramework { get; set; }

    [FromQuery]
    public string? semVerLevel { get; set; }

    [XmlIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public NuGetVersionString NuGetVersion { get => (NuGetVersionString)semVerLevel; }
}
