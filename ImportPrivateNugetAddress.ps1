# file location: '%appdata%/NuGet/Nuget.Config'
# checkout Nuget.Config.example for details
# dotnet nuget push Package.1.0.0.nupkg -s http://localhost:3377/v3/index.json -k "CCBE8511B3D24EFA84CD8CD16C1E6AB0"
dotnet nuget remove source "PrivateNupkgServer source"
dotnet nuget add source "http://localhost:3377/v3/index.json" --name "PrivateNupkgServer source" --username "admin" --password "admin" --store-password-in-clear-text
pausedoc