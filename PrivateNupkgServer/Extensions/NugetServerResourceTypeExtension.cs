using privatenupkgserver.Enums;
using System;

namespace privatenupkgserver
{
    public static class NugetServerResourceTypeExtension
    {
        public static string GetText(this NugetServerResourceType resourceType)
        {
            switch (resourceType)
            {
                case NugetServerResourceType.PackageBaseAddress:
                    return $"{NugetServerResourceType.PackageBaseAddress}/3.0.0";

                case NugetServerResourceType.PackagePublish:
                    return $"{NugetServerResourceType.PackagePublish}/2.0.0";

                case NugetServerResourceType.RegistrationsBaseUrl:
                    return $"{NugetServerResourceType.RegistrationsBaseUrl}";

                case NugetServerResourceType.RegistrationsBaseUrl_3_0_0_beta:
                    return $"{NugetServerResourceType.RegistrationsBaseUrl}/3.0.0-beta";

                case NugetServerResourceType.RegistrationsBaseUrl_3_0_0_rc:
                    return $"{NugetServerResourceType.RegistrationsBaseUrl}/3.0.0-rc";

                case NugetServerResourceType.RegistrationsBaseUrl_3_4_0:
                    return $"{NugetServerResourceType.RegistrationsBaseUrl}/3.4.0";

                case NugetServerResourceType.RegistrationsBaseUrl_3_6_0:
                    return $"{NugetServerResourceType.RegistrationsBaseUrl}/3.6.0";

                case NugetServerResourceType.SearchQueryService:
                    return $"{NugetServerResourceType.SearchQueryService}";

                case NugetServerResourceType.SearchQueryService_3_0_0_beta:
                    return $"{NugetServerResourceType.SearchQueryService}/3.0.0-beta";

                case NugetServerResourceType.SearchQueryService_3_0_0_rc:
                    return $"{NugetServerResourceType.SearchQueryService}/3.0.0-rc";

                default:
                    break;
            }
            throw new NotSupportedException();
        }
    }
}