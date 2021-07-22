# How to publish to Nuget

Use TxFileManagerTest package for Nuget testing.

Create package (in FileManager project)

```
dotnet pack --output nupkgs
dotnet build -c Release
```

Push package

```
dotnet nuget push .\bin\release\TxFileManager.1.5.1.nupkg --api-key $key --source https://api.nuget.org/v3/index.json
```