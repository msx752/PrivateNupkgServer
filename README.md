# PrivateNupkgServer

forked from [MaiReo/nugetserver](https://github.com/MaiReo/nugetserver) and has been revised/modernized to **.NET 6.0** and following new framework features have been applied.
- **(.NET 6, C# 10); Minimal API has been applied
- **(.NET 6, C# 10); new File Scoped Namespaces has been applied**

## Supported Storages

| Types of the Supported Nupkg Storage | Description |
| --- | ----------- |
| LocalDisk | Stores in Physical Environment |
| AzureBlobStorage | requires Azure Storage Account |

## Next Steps
- convert to extension library for each supported storage types
- Distributed Cache Service
- Distributed Search Service
- Tenant for the Organizations
- Download Counter
- UI for the basic management
- Login Page for the UI
