namespace privatenupkgserver.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v2")]
public class V2NugetController : ControllerBase
{
    private readonly IOptions<NugetServerOption> options;

    public V2NugetController(IOptions<NugetServerOption> options) => this.options = options;

    [HttpPut("package")]
    public IActionResult PackagePut([FromRoute] NugetImportModel model) => Redirect($"~{options.Value.GetApiMajorVersionUrl()}/package");
}
