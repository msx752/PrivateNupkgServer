using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using privatenupkgserver.Extensions;
using privatenupkgserver.Models.Nuget;
using privatenupkgserver.Options;

namespace privatenupkgserver.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/v2")]
    public class V2NugetController : ControllerBase
    {
        private readonly IOptions<NugetServerOption> options;

        public V2NugetController(IOptions<NugetServerOption> options) => this.options = options;

        [HttpDelete("package/{nugetId}/{nugetVersion}")]
        public IActionResult PackageDelete(string nugetId, string nugetVersion) => Redirect($"~{options.Value.GetApiMajorVersionUrl()}/package/{nugetId}/{nugetVersion}");

        [HttpGet("package/{nugetId}/{nugetVersion}")]
        public IActionResult PackageGet(string nugetId, string nugetVersion) => Redirect($"~{options.Value.GetApiMajorVersionUrl()}/flatcontainer/{nugetId}/{nugetVersion}/{nugetId}.{nugetVersion}.nupkg");

        [HttpPut("package")]
        public IActionResult PackagePut([FromRoute] NugetImportModel model) => Redirect($"~{options.Value.GetApiMajorVersionUrl()}/package");
    }
}