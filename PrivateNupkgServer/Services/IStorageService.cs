namespace privatenupkgserver.Services;

public interface IStorageService
{
    SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl);

    void DeleteNupkg(string filePath, string nugetId, string nugetVersion);

    Task AddNupkgAsync(string nugetId, string nugetVersion, Stream package);

    string GetPackageRootFullPath();

    string CombineToFilePath(string id, string version);

    string GetNuspecName(string nugetId, string nugetVersion);

    bool ContainsNuspec(string nugetId, string nugetVersion);

    Nuspec GetNuspec(string nugetId, string nugetVersion);

    IEnumerable<string> ListNuspecName();

    IEnumerable<Nuspec> ListNuspecPackage();

    RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInputModel);

    bool IsDeleted(string id, string version);

    Stream GetNupkg(string packageName, string packageVersion);
}
