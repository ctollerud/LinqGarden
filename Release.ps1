param(
    [Parameter(Mandatory=$true)]
    [string]
    $Version
)

$ErrorActionPreference = 'Stop'

$diffResults = git diff
if( $diffResults ) {
    throw "uncommitted changes detected."
}


"<Project>
    <PropertyGroup>
        <Version>$Version</Version>
    </PropertyGroup>
</Project>" | out-file $(Join-Path $PSScriptRoot src/Version.props) -Encoding utf8

./Build.ps1

Write-Host "Build Completed. Committing..."
exec{git add -A }
exec{git commit -m "shipping package version $Version" }

Write-Host "Tagging the versions"
exec{git tag -a "v-$Version" -m "Tagged as nuget package version $Version" }

Write-Host "Version update has been built, committed and tagged.  Final steps:
- Push nuget up nuget packages
- Push new commit (with tags!)
"