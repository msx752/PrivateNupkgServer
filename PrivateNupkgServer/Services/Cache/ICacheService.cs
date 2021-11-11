namespace privatenupkgserver.Services.Cache;

public interface ICacheService
{
    void UpdateCachedNupkg(string Id, string version, byte[] Nupkg);

    Stream GetCachedNupkg(string Id, string version);

    void InitCacheService(Func<IEnumerable<Nuspec>> funcNuspec);

    void UpdateCachedNuspec(string Id, string version, Nuspec nuspec);

    void DeleteCachedNupkg(string Id, string version);

    Nuspec GetCachedNuspec(string Id, string version);

    IEnumerable<Nuspec> GetCachedNuspecs();
}
