using Microsoft.AspNetCore.Http;
using NuGet.Versioning;
using privatenupkgserver.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace privatenupkgserver.Options
{
    public class NugetServerOption
    {
        public NugetServerOption()
        {
            Resources = new Dictionary<NugetServerResourceType, PathString>();
        }

        /// <summary>
        /// Gets or set nuget server api version. Only supports 3.0.0-beta.1 currently.
        /// </summary>
        public string ApiVersion { get; set; }

        public NuGetVersion NugetApiVersion => NuGetVersion.Parse(ApiVersion);

        /// <summary>
        /// For more infomation, please visit https://docs.microsoft.com/en-us/nuget/api/service-index .
        /// </summary>
        public PathString ServiceIndex { get; set; }

        /// <summary>
        /// Gets or set nuget package (.nupkg) directory root path.
        /// Default is "packages".
        /// Case-(in)sensitive depends on platform.
        /// </summary>
        public string PackageDirectory { get; set; }

        /// <summary>
        /// For more infomation, please visit https://docs.microsoft.com/en-us/nuget/api/service-index .
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<NugetServerResourceType, PathString> Resources { get; private set; }

        /// <summary>
        /// Interprets the package delete request as an "unlist".
        /// For more infomation, please visit https://docs.microsoft.com/en-us/nuget/api/package-publish-resource
        /// </summary>
        public bool InterpretUnListEnabled { get; set; }

        public bool CacheEnabled { get; set; }
    }
}