namespace privatenupkgserver.Services.Cache;

public class CacheInMemoryService : ICacheService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Nuspec>> cachedNuspec;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>> cachedNupkg;

    private readonly object lock_cachedNuspec = new object();
    private bool init = false;

    public CacheInMemoryService()
    {
        cachedNuspec = new ConcurrentDictionary<string, ConcurrentDictionary<string, Nuspec>>();
        cachedNupkg = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>>();
    }

    public void InitCacheService(Func<IEnumerable<Nuspec>> funcNuspec)
    {
        if (!init)
        {
            lock (cachedNuspec)
            {
                if (init)
                    return;

                init = true;
                var idGroups = funcNuspec().GroupBy(n => n.Metadata.Id);
                foreach (var idGroup in idGroups)
                {
                    var dictionaryNuspec = idGroup.Select(x => x);
                    var dictionaryVersion = dictionaryNuspec.ToDictionary(x => x.Metadata.Version);
                    var concurrentCachedNuspecDic = new ConcurrentDictionary<string, Nuspec>(dictionaryVersion);
                    cachedNuspec.TryAdd(idGroup.Key.ToLowerInvariant(), concurrentCachedNuspecDic);

                    var concurrentCachedNupkgDic = new ConcurrentDictionary<string, byte[]>();
                    foreach (var item in dictionaryNuspec)
                    {
                        using (var stream = new PhysicalFileInfo(new FileInfo(item.FilePath)).CreateReadStream())
                        {
                            byte[] buff = new byte[(int)stream.Length];
                            stream.Read(buff, 0, buff.Length);
                            concurrentCachedNupkgDic.TryAdd(item.Metadata.Version, buff);
                        }
                    }
                    cachedNupkg.TryAdd(idGroup.Key.ToLowerInvariant(), concurrentCachedNupkgDic);
                }
            }
        }
    }

    public void UpdateCachedNuspec(string Id, string version, Nuspec nuspec)
    {
        var versionDic = cachedNuspec.GetOrAdd(Id.ToLowerInvariant(), id => new ConcurrentDictionary<string, Nuspec>());
        versionDic.AddOrUpdate(version, nuspec, (version, old) => nuspec);
    }

    public void UpdateCachedNupkg(string Id, string version, byte[] Nupkg)
    {
        var versionDic = cachedNupkg.GetOrAdd(Id.ToLowerInvariant(), id => new ConcurrentDictionary<string, byte[]>());
        versionDic.AddOrUpdate(version, Nupkg, (version, old) => Nupkg);
    }

    public void DeleteCachedNupkg(string Id, string version)
    {
        cachedNuspec[Id.ToLowerInvariant()].TryRemove(version, out Nuspec nuspec);
        cachedNupkg[Id.ToLowerInvariant()].TryRemove(version, out byte[] nupkg);
    }

    public Stream? GetCachedNupkg(string Id, string version)
    {
        try
        {
            Stream stream = new MemoryStream(cachedNupkg[Id][version]);
            return stream;
        }
        catch
        {
            return null;
        }
    }

    public Nuspec? GetCachedNuspec(string Id, string version)
    {
        try
        {
            return cachedNuspec[Id.ToLowerInvariant()][version.ToLowerInvariant()];
        }
        catch (Exception)
        {
            return null;
        }
    }

    public IEnumerable<Nuspec> GetCachedNuspecs()
    {
        return cachedNuspec.SelectMany(id => id.Value.Select(v => v.Value));
    }
}
