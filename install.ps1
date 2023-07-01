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

    $InstalledExe = Join-Path $InstallDirectory "WarmCorners.exe"

    return $InstalledExe
}

function New-ShortcutIntoStartup {
    [CmdletBinding(SupportsShouldProcess)]
    param (
        [string]$SourceFilePath
    )

    $ShortcutPath = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\Startup\WarmCorners.lnk"
    
    if ($PSCmdlet.ShouldProcess($Path, ("Setting content to '{0}'" -f $Content))) {
        $WScriptObj = New-Object -ComObject ("WScript.Shell")
        $Shortcut = $WscriptObj.CreateShortcut($ShortcutPath)
        $Shortcut.TargetPath = $SourceFilePath
        $shortcut.Save()
    } else {
        Write-Output("Creating shortcut for `"" + $SourceFilePath + "`" in `"" + $ShortcutPath + "`"")
    }
}

function Remove-DownloadedFile {
    [CmdletBinding(SupportsShouldProcess)]
    param (
        [string]$ReleaseZipFile
    )

    Remove-Item $ReleaseZipFile -WhatIf:$WhatIfPreference
}

$ReleaseZipFile = Get-ReleaseFromGitHub
$ExeFile = Expand-ReleaseZip $ReleaseZipFile
New-ShortcutIntoStartup $ExeFile
Remove-DownloadedFile $ReleaseZipFile
