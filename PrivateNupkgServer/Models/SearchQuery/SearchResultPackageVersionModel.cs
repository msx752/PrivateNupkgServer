namespace privatenupkgserver.Models.SearchQuery;

public class SearchResultPackageVersionModel
{
    public SearchResultPackageVersionModel()
    {
        Id = string.Empty;
        Version = "1.0.0"; //NuGetVersionString.Default;
    }

    [JsonProperty("@id")]
    public string Id { get; set; }

    public string Version { get; set; }

    public int Downloads { get; set; }
}
