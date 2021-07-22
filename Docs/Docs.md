# How to publish to Nuget

Create package (in FileManager project)
```
dotnet pack -c Release --output nupkgs
dotnet build -c Release
```

Push package (don't use the file in the project folder - that one is missing a bunch of attrs for some reason)
```
dotnet nuget push .\bin\release\TxFileManager.1.5.0.nupkg --api-key $key --source https://api.nuget.org/v3/index.json
```
