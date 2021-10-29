using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using privatenupkgserver.Extensions;
using privatenupkgserver.Models.Nuget;
using privatenupkgserver.Models.Nuspec;
using privatenupkgserver.Models.Registration;
using privatenupkgserver.Models.ServerIndex;
using privatenupkgserver.Providers;
using privatenupkgserver.Serializations;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace privatenupkgserver.Controllers
{
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

        [AllowAnonymous]
        [HttpGet("index.json")]
        public IActionResult ServerIndex()
        {
            ServerIndexModel serverIndexModel = nupkgProvider.GetServerIndex(HttpContext.GetBaseUrl());
            return ResponseNugetModel(serverIndexModel, (jsr) => { jsr.ContractResolver = new ServerIndexContractResolver(); jsr.Formatting = Formatting.Indented; });
        }

        [HttpGet("query")]
        public IActionResult Query([FromRoute] NugetQueryServiceModel queryModel)
        {
            return ResponseNugetModel(nupkgProvider.SearchQuery(queryModel, HttpContext.GetBaseUrl()));
        }

        [HttpGet("flatcontainer/{packageName}/{packageVersion}/{nupkgName:regex(([[a-zA-Z0-9]]+[[\\.]])+)}.nupkg")]
        public IActionResult FlatcontainerNupkg(string packageName, string packageVersion, string nupkgName)
        {
            var nupkg = nupkgProvider.GetNupkg(packageName, packageVersion);
            if (nupkg == null)
                return NotFound();

            return new FileStreamResult(nupkg, "application/octet-stream");
        }

        [HttpGet("flatcontainer/{packageName}/index.json")]
        public IActionResult FlatcontainerVersions(string packageName)
        {
            return ResponseNugetModel(nupkgProvider.GetNugetIdVersions(packageName));
        }

        [HttpGet("registration3/{packageName}/index.json")]
        public IActionResult Registration3(string packageName)
        {
            var nuspecs = nupkgProvider.ListNuspec()
                .Where(nuspec => nuspec.Metadata != null)
                .Where(n => n.Metadata.Id.ToLowerInvariant() == packageName.ToLowerInvariant());
            if (!nuspecs.Any())
                return NotFound();

            return ResponseNugetModel(nupkgProvider.GetRegistrationIndex(new RegistrationInputModel(this.HttpContext.GetBaseUrl(), this.HttpContext.Request.Path)));
        }

        [HttpDelete("package/{packageName}/{packageVersion}")]
        public IActionResult PackageDelete(string packageName, string packageVersion)
        {
            Nuspec nuspec = nupkgProvider.GetNuspec(packageName, packageVersion);
            if (nuspec == null)
                return NotFound();

            nupkgProvider.DeleteNupkg(nuspec.FilePath, packageName, packageVersion);
            return Ok();
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

        private IActionResult ResponseNugetModel(object objModel, Action<JsonSerializerSettings> funcJsonSerializerSettings = null)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            funcJsonSerializerSettings?.Invoke(jsonSerializerSettings);
            return Content(JsonConvert.SerializeObject(objModel, jsonSerializerSettings), new MediaTypeHeaderValue("application/json"));
        }
    }
}