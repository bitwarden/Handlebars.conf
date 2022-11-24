param (
    [string] $task = "",
    [string] $os = "",
    [string] $output = "",
    [string] $ref = "",
    [string] $event = ""
)

# Setup

$scriptPath = $MyInvocation.MyCommand.Path
$dir = Split-Path -Parent $MyInvocation.MyCommand.Path
if ($output -eq "") {
    $output = "${dir}\build"
}

$assemblyVersion = "1.0.0"
$versionSuffix = "dev"
$version = "$assemblyVersion-$versionSuffix"
if ($event -eq "release" -and $ref -match "(?<Version>\d+\.\d+\.?(\d+)?)") {
    $assemblyVersion = $ref.Trim("v")
    $version = $assemblyVersion
}

echo "Version: $version"

$win_rids = @("win-x64", "win-x86")
$lin_rids = @("linux-x64", "linux-arm", "linux-arm64", "linux-musl-x64", "linux-musl-arm", "linux-musl-arm64")
$mac_rids = @("osx-x64", "osx.11.0-arm64", "osx.12-arm64")

# Functions

function GetOsRids() {
    $rids = @()
    if ($os -eq "win") {
        $rids = $win_rids
    }
    elseif ($os -eq "lin") {
        $rids = $lin_rids
    }
    elseif ($os -eq "mac") {
        $rids = $mac_rids
    }
    else {
        echo "ERROR: ``os`` param should be win, lin, or mac."
    }
    $rids
}

function BuildSelfContainedBinary() {
    $rids = GetOsRids
    foreach ($rid in $rids) {
        $o = "$output\sc\$rid"
        Remove-Item -LiteralPath $o -Force -Recurse -ErrorAction Ignore

        echo "### Building self contained binary for $rid to $o"
        dotnet publish -c Release -o $o -r $rid `
            -p:PublishReadyToRun=true -p:PublishSingleFile=true `
            -p:DebugType=None -p:DebugSymbols=false -p:PublishTrimmed=true `
            --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true `
            -p:EnableCompressionInSingleFile=true `
            -p:AssemblyVersion=$assemblyVersion -p:Version=$version -p:VersionSuffix=$versionSuffix
    }
}

function BuildFrameworkDependentBinary() {
    $rids = GetOsRids
    foreach ($rid in $rids) {
        $o = "$output\fd\$rid"
        Remove-Item -LiteralPath $o -Force -Recurse -ErrorAction Ignore

        echo "### Building framework dependent binary for $rid to $o"
        dotnet publish -c Release -o $o -r $rid `
            -p:PublishReadyToRun=true -p:PublishSingleFile=true `
            -p:DebugType=None -p:DebugSymbols=false `
            --self-contained false -p:IncludeNativeLibrariesForSelfExtract=true `
            -p:AssemblyVersion=$assemblyVersion -p:Version=$version -p:VersionSuffix=$versionSuffix
    }
}

function Clean() {
    Remove-Item -LiteralPath $output -Force -Recurse -ErrorAction Ignore
}

# Execute

echo "## Building Handlebars.conf ($task)"

if ($task -eq "binary-sc") {
    BuildSelfContainedBinary
}
elseif ($task -eq "binary-fd") {
    BuildFrameworkDependentBinary
}
elseif ($task -eq "clean") {
    Clean
}
else {
    echo "ERROR: ``task`` param should be binary or clean."
}