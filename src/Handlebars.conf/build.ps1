dotnet publish --configuration Release --runtime win-x64 `
    -p:PublishReadyToRun=true -p:PublishSingleFile=true `
    -p:DebugType=None -p:DebugSymbols=false -p:PublishTrimmed=true `
    --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true