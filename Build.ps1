<#
    Runs tests then builds the nuget package.
#>
$ErrorActionPreference = 'STOP'

function global:exec {
    param (
        [Parameter(Mandatory=$true)]
        [ScriptBlock]$Script
    )

    Invoke-Command $Script

    if($LASTEXITCODE -ne 0) {
        throw "process '$Script' exited with code $LASTEXITCODE"
    }
}
#####BEGIN SCRIPT#####
$solutionPath = Join-Path $PSScriptRoot src/LinqGarden.sln


Write-Host "Running Tests:"
exec{dotnet test $solutionPath}

$linqGardenProjectPath = Join-Path $PSScriptRoot "src/LinqGarden/LinqGarden.csproj"
$outputPath = Join-Path $PSScriptRoot "output/LinqGarden"
Write-Host "Building LinqGarden to dir '$outputPath'..."

exec{dotnet build $linqGardenProjectPath -c Release -o $outputPath}