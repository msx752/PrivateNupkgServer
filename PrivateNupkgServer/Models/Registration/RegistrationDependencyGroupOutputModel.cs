namespace privatenupkgserver.Models.Registration;

public class RegistrationDependencyGroupOutputModel
{
    public RegistrationDependencyGroupOutputModel()
    {
        Dependencies = new List<RegistrationDependencyOutputModel>(0);
    }

    [JsonProperty("@id")]
    public string Id_alias { get; set; }

    public string TargetFramework { get; set; }

    public List<RegistrationDependencyOutputModel> Dependencies { get; set; }
}
