var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "privatenupkgserver", Version = "v1" }));
builder.Services.AddNugetServer(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(c => c.SerializeAsV2 = false);
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "privatenupkgserver v1"));
}

app.UseRouting();
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();
app.UseNugetServer();
app.UseEndpoints(endpoints => endpoints.MapControllers());

var putPackage =[Authorize] async (HttpRequest req, [FromServices] INupkgProvider nupkgProvider) =>
{
    if (!req.HasFormContentType)
        return Results.BadRequest();

    var form = req.Form.Files.FirstOrDefault();
    if (form is null)
        return Results.BadRequest();

    using (Stream nstream = form.OpenReadStream())
    {
        Nuspec nuspec = await Zip.ReadNuspecFromPackageAsync(nstream);
        if (nuspec?.Metadata == null)
            return Results.BadRequest();

        if (nupkgProvider.ContainsNuspec(nuspec.Metadata.Id, nuspec.Metadata.Version))
            return Results.Conflict();

        nstream.Seek(0, SeekOrigin.Begin);
        await nupkgProvider.AddNupkgAsync(nuspec.Metadata.Id, nuspec.Metadata.Version, nstream);
        return Results.StatusCode((int)HttpStatusCode.Created);
    }
};

app.MapGet("/", [AllowAnonymous] () => ".NET 6.0 server is running!");

app.MapDelete("api/v2/package/{nugetId}/{nugetVersion}", [AllowAnonymous] ([FromServices] IOptions<NugetServerOption> options, HttpContext http, string nugetId, string nugetVersion) =>
    http.Response.Redirect($"~{options.Value.GetApiMajorVersionUrl()}/package/{nugetId}/{nugetVersion}"));

app.MapGet("api/v2/package/{nugetId}/{nugetVersion}", [AllowAnonymous] ([FromServices] IOptions<NugetServerOption> options, HttpContext http, string nugetId, string nugetVersion) =>
    http.Response.Redirect($"~{options.Value.GetApiMajorVersionUrl()}/flatcontainer/{nugetId}/{nugetVersion}/{nugetId}.{nugetVersion}.nupkg"));

app.MapPut("api/v2/package", putPackage);

app.MapGet("v3/index.json", [AllowAnonymous] ([FromServices] INupkgProvider nupkgProvider, HttpContext http) =>
     ResponseNugetModel(nupkgProvider.GetServerIndex(http.GetBaseUrl()), (jsr) => { jsr.ContractResolver = new ServerIndexContractResolver(); jsr.Formatting = Newtonsoft.Json.Formatting.Indented; }));

app.MapGet("v3/query", [Authorize] ([FromServices] INupkgProvider nupkgProvider, HttpContext http) =>
    ResponseNugetModel(nupkgProvider.SearchQuery(new NugetQueryServiceModel(http.Request.Query), http.GetBaseUrl())));

app.MapGet("v3/flatcontainer/{packageName}/{packageVersion}/{nupkgName:regex(([[a-zA-Z0-9]]+[[\\.]])+)}.nupkg", [Authorize] ([FromServices] INupkgProvider nupkgProvider, string packageName, string packageVersion, string nupkgName) =>
{
    var nupkg = nupkgProvider.GetNupkg(packageName, packageVersion);
    if (nupkg == null)
        return Results.NotFound();
    return Results.Stream(nupkg, "application/octet-stream");
});

app.MapGet("v3/registration3/{packageName}/index.json", [Authorize] ([FromServices] INupkgProvider nupkgProvider, HttpContext http, string packageName) =>
{
    var nuspecs = nupkgProvider.ListNuspec()
        .Where(nuspec => nuspec.Metadata != null)
        .Where(n => n.Metadata.Id.ToLowerInvariant() == packageName.ToLowerInvariant());
    if (!nuspecs.Any())
        return Results.NotFound();

    return ResponseNugetModel(nupkgProvider.GetRegistrationIndex(new RegistrationInputModel(http.GetBaseUrl(), http.Request.Path)));
});

app.MapGet("v3/flatcontainer/{packageName}/index.json", [Authorize] ([FromServices] INupkgProvider nupkgProvider, string packageName) => ResponseNugetModel(nupkgProvider.GetNugetIdVersions(packageName)));

app.MapDelete("v3/package/{packageName}/{packageVersion}", [Authorize] ([FromServices] INupkgProvider nupkgProvider, string packageName, string packageVersion) =>
{
    Nuspec nuspec = nupkgProvider.GetNuspec(packageName, packageVersion);
    if (nuspec == null)
        return Results.NotFound();

    nupkgProvider.DeleteNupkg(nuspec.FilePath, packageName, packageVersion);
    return Results.Ok();
});

app.MapPut("v3/package", putPackage);

IResult ResponseNugetModel(object objModel, Action<JsonSerializerSettings>? funcJsonSerializerSettings = null)
{
    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Newtonsoft.Json.Formatting.Indented,
    };
    funcJsonSerializerSettings?.Invoke(jsonSerializerSettings);
    return Results.Content(JsonConvert.SerializeObject(objModel, jsonSerializerSettings), new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
}

app.Run();