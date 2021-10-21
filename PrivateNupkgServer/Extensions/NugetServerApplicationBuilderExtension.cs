using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using privatenupkgserver.Enums;
using privatenupkgserver.Middlewares;
using privatenupkgserver.Options;

namespace privatenupkgserver.Extensions
{
    public static class NugetServerApplicationBuilderExtension
    {
        public static IApplicationBuilder UseNugetServer(this IApplicationBuilder app)
        {
            app.UseExceptionHandlerMiddleware();

            var nugetServerOption = app.ApplicationServices.GetRequiredService<IOptions<NugetServerOption>>();
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.PackageBaseAddress, "/flatcontainer");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.PackagePublish, "/package");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.SearchQueryService, "/query");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.SearchQueryService_3_0_0_beta, "/query");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.SearchQueryService_3_0_0_rc, "/query");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.RegistrationsBaseUrl, "/registration3");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.RegistrationsBaseUrl_3_0_0_beta, "/registration3");
            nugetServerOption.Value.Resources.Add(NugetServerResourceType.RegistrationsBaseUrl_3_0_0_rc, "/registration3");

            return app;
        }
    }
}