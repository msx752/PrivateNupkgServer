namespace privatenupkgserver.Models.Nuget;

public class NugetQueryServiceModel
{
    public NugetQueryServiceModel()
    {
    }

    public NugetQueryServiceModel(IQueryCollection queryCollection)
    {
        q = queryCollection[nameof(q)];
        skip = int.Parse(queryCollection[nameof(skip)]);
        take = int.Parse(queryCollection[nameof(take)]);
        prerelease = bool.Parse(queryCollection[nameof(prerelease)]);
        semVerLevel = queryCollection[nameof(semVerLevel)];
        supportedFramework = queryCollection[nameof(supportedFramework)];
    }

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