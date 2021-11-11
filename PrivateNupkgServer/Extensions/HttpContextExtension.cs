namespace privatenupkgserver.Extensions;

public static class HttpContextExtension
{
    /// <summary>
    /// How to get the correct base url
    /// when app is NOT hosting at the root path "/" ?
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetBaseUrl(this HttpContext context)
    {
        return context.Request.Scheme + "://" + context.Request.Host;
    }
}
