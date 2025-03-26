<# CalibreImportShellExtension Setup file.
    This script registers all CalibreImport Shell Extension DLL files;
    Registration creates context menu handlers for Calibre file extensions.
    The script can be run with the following arguments:
    -i: Register the DLLs and add registry entries.
    -u: Unregister the DLLs and remove registry entries.
    -re: Restart Windows Explorer.
    If no arguments are provided, the script will default to the Install action.
#>

param (
    [switch]$i, # install
    [switch]$u, # uninstall
    [switch]$re # restart explorer
)

# variables
$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path -Parent $scriptPath
$logFilePath = Join-Path $scriptDir "Setup.log"
$regasmPath64 = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe"
$regasmPath32 = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe"

function Log {
    param (
        [string]$message
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Add-Content -Path $logFilePath -Value "$timestamp - $message"
}

# register the DLL using regasm.exe
function Register-Dll {
    param (
        [string]$dllPath
    )
    try {
        if (Test-Path $regasmPath64) {
            & $regasmPath64 /codebase $dllPath
        } elseif (Test-Path $regasmPath32) {
            & $regasmPath32 /codebase $dllPath
        } else {
            throw "Regasm.exe not found. Is .Net framework 4.8 installed?"
        }
        Log "Registered DLL: $dllPath"
    } catch {
        Log "Error registering DLL: $dllPath - $_"
    }
}

# unregister the DLL using regasm.exe
function Unregister-Dll {
    param (
        [string]$dllPath
    )
    try {
        if (Test-Path $regasmPath64) {
            & $regasmPath64 /unregister $dllPath
        } elseif (Test-Path $regasmPath32) {
            & $regasmPath32 /unregister $dllPath
        } else {
            throw "Regasm.exe not found. Is .Net framework 4.8 installed?"
        }
        Log "Unregistered DLL: $dllPath"
    } catch {
        Log "Error unregistering DLL: $dllPath - $_"
    }
}

# full list of Calibre-supported extensions from the registry
function GetCalibreExtensions {
    $calibreAssocKey = "HKCU:\SOFTWARE\calibre\calibre64bit\Capabilities\FileAssociations"
    if (Test-Path $calibreAssocKey) {
        return (Get-ItemProperty -Path $calibreAssocKey | Get-Member -MemberType NoteProperty).Name |
            ForEach-Object { "$($_)" }
    } else {
        Write-Host "Calibre file association registry key not found. Using default extensions." -ForegroundColor Yellow
        return @(".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu", ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html", ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar")
    }
}

<# adding registry entries for all Calibre-supported extensions
this is redundant, since the C# code already takes care of this upon registration #>
function Add-RegistryEntries {
    $extensions = GetCalibreExtensions
    $guid = "8E5CD5CA-64E0-479A-B62F-B1FC00FF0227"

    foreach ($ext in $extensions) {
        $keyPath = "HKCU:\SOFTWARE\Classes\SystemFileAssociations\$ext\Shellex\ContextMenuHandlers\CalibreContextMenuExtension"
        if (-not (Test-Path $keyPath)) {
            New-Item -Path $keyPath -Force | Out-Null
        }
        Set-ItemProperty -Path $keyPath -Name "(default)" -Value "{$guid}"
        Log "Added registry entry for extension: $ext"
    }
}

<# removing registry entries for all Calibre-supported extensions
this is redundant, since the C# code already takes care of this upon unregistration #>
function Remove-RegistryEntries {
    $extensions = GetCalibreExtensions

    foreach ($ext in $extensions) {
        $keyPath = "HKCU:\SOFTWARE\Classes\SystemFileAssociations\$ext\Shellex\ContextMenuHandlers\CalibreContextMenuExtension"
        if (Test-Path $keyPath) {
            Remove-Item -Path $keyPath -Recurse -Force
            Log "Removed registry entry for extension: $ext"
        }
    }
}

# optional
function Restart-Explorer {
    Stop-Process -Name explorer -Force
    Start-Process explorer
    Log "Explorer restarted"
}

# verify whether the dll registration went through successfully
function Check-DllRegistration {
    param (
        [string]$dllPath
    )
    $dllName = [System.IO.Path]::GetFileNameWithoutExtension($dllPath)
    $clsidKeyPath = "HKCR:\CLSID"
    $guid = "8E5CD5CA-64E0-479A-B62F-B1FC00FF0227"

    $keyPath = Join-Path $clsidKeyPath $guid
    Log "Checking registry path: $keyPath"
    if (Test-Path $keyPath) {
        $message = "DLL is registered: $dllPath"
        Log $message
        return $message
    } else {
        $message = "DLL is not registered: $dllPath"
        Log $message
        return $message
    }
}

# check for DLL files in the script directory
$dllFiles = Get-ChildItem -Path $scriptDir -Filter *.dll
if (-not $dllFiles) {
    Log "No DLLs found in the script directory."
    throw "No DLLs found in the script directory."
}

# main script logic
if ($i -or (-not $u -and -not $re)) {
    foreach ($dllFile in $dllFiles) {
        $dllPath = $dllFile.FullName
        Register-Dll -dllPath $dllPath
        $registrationMessage = Check-DllRegistration -dllPath $dllPath
        Write-Output $registrationMessage
    }
    Add-RegistryEntries
    if ($re) {
        Restart-Explorer
    } else {
        $response = Read-Host "Do you want to restart Explorer now? (y/n)"
        if ($response -eq "y") {
            Restart-Explorer
        }
    }
} elseif ($u) {
    foreach ($dllFile in $dllFiles) {
        $dllPath = $dllFile.FullName
        Unregister-Dll -dllPath $dllPath
        $registrationMessage = Check-DllRegistration -dllPath $dllPath
        Write-Output $registrationMessage
    }
    Remove-RegistryEntries
    if ($re) {
        Restart-Explorer
    } else {
        $response = Read-Host "Do you want to restart Explorer now? (y/n)"
        if ($response -eq "y") {
            Restart-Explorer
        }
    }
} elseif ($re) {
    Restart-Explorer
} else {
    # No need to specify an argument message, default to Install
    foreach ($dllFile in $dllFiles) {
        $dllPath = $dllFile.FullName
        Register-Dll -dllPath $dllPath
        $registrationMessage = Check-DllRegistration -dllPath $dllPath
        Write-Output $registrationMessage
    }
    Add-RegistryEntries
    if ($re) {
        Restart-Explorer
    } else {
        $response = Read-Host "Do you want to restart Explorer now? (y/n)"
        if ($response -eq "y") {
            Restart-Explorer
        }
    }
}
