using Microsoft.AspNetCore.Http;
using NuGet.Versioning;
using System;

namespace privatenupkgserver.Models.Registration
{
    public class RegistrationInputModel
    {
        public RegistrationInputModel(string baseUrl, PathString path)
        {
            BaseUrl = baseUrl;
            Path = path;
            _splitedPaths = Path.ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private readonly string[] _splitedPaths;

        public string BaseUrl { get; private set; }

        public PathString Path { get; private set; }

        public PathString PathBase { get; set; }

        public string Id { get => GetId(); }

        public NuGetVersion Version { get => GetVersion(); }

        private string GetId()
        {
            if (_splitedPaths.Length < 3)
            {
                throw new InvalidOperationException();
            }
            return _splitedPaths[2].ToLowerInvariant();
        }

        private NuGetVersion GetVersion()
        {
            if (_splitedPaths.Length < 4)
            {
                throw new InvalidOperationException();
            }
            var dotJsonPos = _splitedPaths[3].LastIndexOf(".json");
            if (dotJsonPos == -1)
            {
                return null;
            }
            var nameOrVersion =
                _splitedPaths[3].Substring(0, dotJsonPos).ToLowerInvariant();
            if (string.Equals(nameOrVersion, "index"))
            {
                return null;
            }
            if (NuGetVersion.TryParse(nameOrVersion, out var version))
            {
                return version;
            }
            return null;
        }

        private NuGetVersion GetPageStartVersion()
        {
            if (_splitedPaths.Length < 6)
            {
                throw new InvalidOperationException();
            }

            var versionString = _splitedPaths[4].ToLowerInvariant();

            if (NuGetVersion.TryParse(versionString, out var version))
            {
                return version;
            }
            return null;
        }

        private NuGetVersion GetPageEndVersion()
        {
            if (_splitedPaths.Length < 6)
            {
                throw new InvalidOperationException();
            }
            var dotJsonPos = _splitedPaths[5].LastIndexOf(".json");
            if (dotJsonPos == -1)
            {
                return null;
            }
            var lower = _splitedPaths[5].Substring(0, dotJsonPos).ToLowerInvariant();

            if (NuGetVersion.TryParse(lower, out var version))
            {
                return version;
            }
            return null;
        }
    }
}