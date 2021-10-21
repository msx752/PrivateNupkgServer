using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using privatenupkgserver.Handlers;
using privatenupkgserver.Options;
using privatenupkgserver.Providers;
using privatenupkgserver.Services.AzureBlobStorage;
using privatenupkgserver.Services.Cache;
using privatenupkgserver.Services.LocalDisk;

namespace privatenupkgserver.Extensions
{
    public static class NugetServerCoreServiceCollectionExtension
    {
        public static IServiceCollection AddNugetServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
            services.Configure<NugetServerOption>(configuration.GetSection(nameof(NugetServerOption)));
            services.Configure<NugetServerCredentialOption>(configuration.GetSection(nameof(NugetServerCredentialOption)));
            services.Configure<NugetServerStorageOption>(configuration.GetSection(nameof(NugetServerStorageOption)));

            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, NupkgAuthenticationHandler>("BasicAuthentication", null);

            services.AddSingleton<ICacheService, CacheInMemoryService>();
            services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddScoped<ILocalDiskStorageService, LocalDiskStorageService>();
            services.AddScoped<INupkgProvider, NupkgProvider>();

            return services;
        }
    }
}