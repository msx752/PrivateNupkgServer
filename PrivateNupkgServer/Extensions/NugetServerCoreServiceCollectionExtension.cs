namespace privatenupkgserver.Extensions;

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
