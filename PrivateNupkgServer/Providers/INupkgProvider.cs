using privatenupkgserver.Models.Nuget;
using privatenupkgserver.Models.Nuspec;
using privatenupkgserver.Models.PackageBaseAddress;
using privatenupkgserver.Models.Registration;
using privatenupkgserver.Models.SearchQuery;
using privatenupkgserver.Models.ServerIndex;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace privatenupkgserver.Providers
{
    public interface INupkgProvider
    {
        IEnumerable<Nuspec> ListNuspec();

        PackageBaseAddressVersionsOutputModel GetNugetIdVersions(string nugetId);

        SearchOutputModel SearchQuery(NugetQueryServiceModel nugetQueryModel, string baseUrl);

        Stream GetNupkg(string nugetId, string nugetVersion);

        Task AddNupkgAsync(string nugetId, string nugetVersion, Stream nuspecStream);

        string GetNuspecName(string nugetId, string nugetVersion);

        Nuspec GetNuspec(string nugetId, string nugetVersion);

        bool ContainsNuspec(string nugetId, string nugetVersion);

        void DeleteNupkg(string filePath, string nugetId, string nugetVersion);

        ServerIndexModel GetServerIndex(string baseUrl);

        RegistrationIndexOutputModel GetRegistrationIndex(RegistrationInputModel registrationInputModel);
    }
}