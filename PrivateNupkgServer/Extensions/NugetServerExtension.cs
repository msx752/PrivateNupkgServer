namespace privatenupkgserver.Extensions;

public static class NugetServerExtension
{
    public static PathString GetResourceUrlPath(this NugetServerOption provider, NugetServerResourceType resourceType)
    {
        var majorVersion = provider.GetApiMajorVersionUrl();

        var path = provider.GetResourceValue(resourceType);
        if (!path.HasValue)
        {
            throw new InvalidOperationException("Nuget server resource " + resourceType + " not specified.");
        }
        return majorVersion + path;
    }

    public static Newtonsoft.Json.JsonSerializer CreateJsonSerializerForServiceIndex()
    {
        var serializer = CreateJsonSerializer();
        serializer.ContractResolver = new ServerIndexContractResolver();
        return serializer;
    }

    private static Newtonsoft.Json.JsonSerializer CreateJsonSerializer()
    {
        return new Newtonsoft.Json.JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public static PathString GetApiMajorVersionUrl(this NugetServerOption provider)
    {
        var majorVersion = provider?.NugetApiVersion?.Major;
        if (!majorVersion.HasValue)
        {
            throw new InvalidOperationException("Nuget server api version not specified.");
        }
        return "/v" + majorVersion;
    }

    public static PathString GetResourceValue(this NugetServerOption provider, NugetServerResourceType resourceType)
    {
        return provider?.Resources?.Any(f => f.Key.Equals(resourceType)) != true ? null : provider.Resources[resourceType];
    }
}
