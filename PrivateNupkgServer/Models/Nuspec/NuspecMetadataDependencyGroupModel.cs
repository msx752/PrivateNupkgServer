namespace privatenupkgserver.Models.Nuspec;

[Serializable]
public class NuspecMetadataDependencyGroupModel
{
    [XmlAttribute("targetFramework")]
    public string? TargetFramework { get; set; }

    [XmlElement("dependency")]
    public List<NuspecMetadataDependencyModel>? Dependency { get; set; }
}
