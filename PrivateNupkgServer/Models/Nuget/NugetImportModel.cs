namespace privatenupkgserver.Models.Nuget;

public class NugetImportModel
{
    [Required]
    public IFormFile? Package { get; set; }
}
