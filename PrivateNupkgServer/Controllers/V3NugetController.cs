namespace privatenupkgserver.Controllers;

[Authorize]
[ApiController]
[Route("v3")]
public class V3NugetController : ControllerBase
{
    private readonly INupkgProvider nupkgProvider;

    public V3NugetController(
        INupkgProvider nupkgProvider
        )
    {
        this.nupkgProvider = nupkgProvider;
    }

    [HttpGet("query")]
    public IActionResult Query([FromRoute] NugetQueryServiceModel queryModel)
    {
        return ResponseNugetModel(nupkgProvider.SearchQuery(queryModel, HttpContext.GetBaseUrl()));
    }

    [HttpPut("package")]
    public async Task<IActionResult> PackagePut([FromRoute] NugetImportModel model)
    {
        if (!this.ModelState.IsValid)
            return BadRequest();

        using (Stream nstream = model.Package.OpenReadStream())
        {
            Nuspec nuspec = await Zip.ReadNuspecFromPackageAsync(nstream);
            if (nuspec?.Metadata == null)
                return BadRequest();

            if (nupkgProvider.ContainsNuspec(nuspec.Metadata.Id, nuspec.Metadata.Version))
                return Conflict();

            nstream.Seek(0, SeekOrigin.Begin);
            await nupkgProvider.AddNupkgAsync(nuspec.Metadata.Id, nuspec.Metadata.Version, nstream);
            return StatusCode((int)HttpStatusCode.Created);
        }
    }

    private IActionResult ResponseNugetModel(object objModel, Action<JsonSerializerSettings>? funcJsonSerializerSettings = null)
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
        };
        funcJsonSerializerSettings?.Invoke(jsonSerializerSettings);
        return Content(JsonConvert.SerializeObject(objModel, jsonSerializerSettings), new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
    }
}
