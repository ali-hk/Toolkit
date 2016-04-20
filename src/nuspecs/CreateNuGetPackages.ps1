$releaseNotesUri = 'https://github.com/ali-hk/Toolkit/wiki/Release-Notes-1.0.0-alpha'
$versionSuffix = '-alpha'
$toolkitVersion = '1.0.0'

$nugetFileName = 'nuget.exe'

if (!(Test-Path $nugetFileName))
{
    Write-Host 'Downloading Nuget.exe ...'

    (New-Object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe', $nugetFileName)
}


$projects = 'Toolkit.Behaviors', 'Toolkit.Collections', 'Toolkit.Collections', 'Toolkit.Common', 'Toolkit.Prism', 'Toolkit.Tasks', 'Toolkit.Uwp', 'Toolkit.Web', 'Toolkit.Xaml'

foreach ($project in $projects)
{
    $coreNuspecPath = "$project.nuspec"
    $coreAssemblyPath = "../$project/bin/Release/$project.dll"
    if ((Test-Path $coreAssemblyPath))
    {
        $fileInfo = Get-Item $coreAssemblyPath
        $coreFileVersion = $fileInfo.VersionInfo.ProductVersion
        $coreFileVersion = $coreFileVersion.Substring(0, $coreFileVersion.LastIndexOf('.'))
        $coreFileVersion = "$coreFileVersion$versionSuffix"

        Invoke-Expression ".\$($nugetFileName) pack $($coreNuspecPath) -Prop releaseNotes=$($releaseNotesUri) -Version $($coreFileVersion)" 
    }
    else
    {
        Write-Host "$project.dll not found"
    }
}

#### Toolkit (Complete) ####

$coreNuspecPath = 'Toolkit.nuspec'

$toolkitVersion = "$toolkitVersion$versionSuffix"

Invoke-Expression ".\$($nugetFileName) pack $($coreNuspecPath) -Prop releaseNotes=$($releaseNotesUri) -Version $($toolkitVersion)" 

Read-Host 'Press Enter to continue'
