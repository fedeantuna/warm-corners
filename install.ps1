#!/usr/bin/env pwsh

function Get-OSArchitecture {
    return [Environment]::Is64BitOperatingSystem ? "x64" : "x86"
}

function Get-ShouldUseSelfContained {
    try {
        $dotnetRuntimesCount = (dotnet --list-runtimes | Select-String -Pattern WindowsDesktop | Select-String -Pattern 7.0.* | Measure-Object -Line).Count
    
        if ($dotnetRuntimesCount -gt 0) {
            return $false
        } else {
            return $true
        }
    }
    catch {
        return $true
    }
}

function Get-ReleaseFromGitHub {
    $Architecture = Get-OSArchitecture
    $FileSuffix = (Get-ShouldUseSelfContained) ? "_sc" : ""
    $ReleaseUrl = "https://github.com/fedeantuna/warm-corners/releases/latest/download/warm-corners_${Architecture}${FileSuffix}.zip"

    $DownloadDirectory = [System.IO.Path]::GetTempPath()
    $RandomFileName = [System.IO.Path]::GetRandomFileName()
    $DownloadFilePath = Join-Path $DownloadDirectory $RandomFileName
    Invoke-WebRequest $ReleaseUrl -OutFile $DownloadFilePath

    return $DownloadFilePath
}

function Expand-ReleaseZip {
    param (
        [string]$ReleaseZipFile
    )

    $InstallDirectory = Join-Path $env:APPDATA "WarmCorners"
    Expand-Archive $ReleaseZipFile -DestinationPath $InstallDirectory
}

function Remove-DownloadedFile {
    [CmdletBinding(SupportsShouldProcess)]
    param (
        [string]$ReleaseZipFile
    )

    Remove-Item $ReleaseZipFile -WhatIf:$WhatIfPreference
}

$ReleaseZipFile = Get-ReleaseFromGitHub
Expand-ReleaseZip $ReleaseZipFile
Remove-DownloadedFile $ReleaseZipFile
