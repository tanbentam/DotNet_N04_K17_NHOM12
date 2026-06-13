$ErrorActionPreference = "Stop"

$vsWhere = Join-Path ${env:ProgramFiles(x86)} `
    "Microsoft Visual Studio\Installer\vswhere.exe"

if (-not (Test-Path $vsWhere)) {
    throw "Cannot find vswhere.exe. Install Visual Studio with MSBuild and VSTest."
}

$msBuild = & $vsWhere `
    -latest `
    -products * `
    -requires Microsoft.Component.MSBuild `
    -find "MSBuild\**\Bin\MSBuild.exe" |
    Select-Object -First 1

$vsTest = & $vsWhere `
    -latest `
    -products * `
    -find "Common7\IDE\Extensions\TestPlatform\vstest.console.exe" |
    Select-Object -First 1

if (-not $msBuild -or -not $vsTest) {
    throw "Cannot find MSBuild or VSTest in the current Visual Studio installation."
}

$appProject = Join-Path $PSScriptRoot "TravelApp\TravelApp.csproj"
$testProject = Join-Path $PSScriptRoot `
    "TravelApp.Tests\TravelApp.Tests.csproj"
$testAssembly = Join-Path $PSScriptRoot `
    "TravelApp.Tests\bin\Debug\net472\TravelApp.Tests.dll"

& $msBuild $appProject `
    /t:Build `
    /p:Configuration=Debug `
    /restore `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    throw "TravelApp build failed."
}

& $msBuild $testProject `
    /t:Build `
    /p:Configuration=Debug `
    /restore `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    throw "TravelApp.Tests build failed."
}

& $vsTest $testAssembly

if ($LASTEXITCODE -ne 0) {
    throw "One or more unit tests failed."
}
