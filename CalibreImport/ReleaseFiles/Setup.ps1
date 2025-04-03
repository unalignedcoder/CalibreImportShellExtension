<# Calibre Import "Portable" Setup file.
    This script registers all CalibreImport Shell Extension DLL files;
    It assumes the location of the dll and Json config file
    in the same directory in which the script is located.
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

# ========== Setup Variables ===========

# Was the dll Repacked? Repacking may happen in Post-Build.ps1
$isrepacked = $false

# Assembly Name
$assemblyName = "CalibreImport" # Change this if the Assembly name changes.

# Paths to regasm.exe for 64-bit and 32-bit
$regasmPath64 = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe"
$regasmPath32 = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe"

# ========== Setup Log ===========

#Log or no log
$EnableLogging = $true

# Define the log file path
$logFilePath = Join-Path $PSScriptRoot "Setup.log"

# Function to log messages
function Log-Message {

    param (
        [string]$message,
        [string]$logFilePath = $logFilePath
    )

    if ($EnableLogging) {

        $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        $logMessage = "$timestamp - $message"
        Add-Content -Path $logFilePath -Value $logMessage
    }
}

# ========== DLL Functions ===========

# register the DLL using Regasm.exe
function Register-Dll {
    param (
        [string]$dllPath
    )

    try {

        Log-Message "Attempting to register DLL: $dllPath", $logFilePath
        Write-Output "Attempting to register DLL: $dllPath"

        $processInfo = New-Object System.Diagnostics.ProcessStartInfo

        if (Test-Path $regasmPath64) {

            $processInfo.FileName = $regasmPath64
            Log-Message "Using 64-bit regasm.exe", $logFilePath

        } elseif (Test-Path $regasmPath32) {

            $processInfo.FileName = $regasmPath32
            Log-Message "Using 32-bit regasm.exe", $logFilePath

        } else {
            Log-Message "Regasm.exe not found. Is .Net framework 4.8 installed?", $logFilePath
            throw "Regasm.exe not found. Is .Net framework 4.8 installed?"
        }

        $processInfo.Arguments = "/codebase $dllPath"
        $processInfo.RedirectStandardOutput = $true
        $processInfo.RedirectStandardError = $true
        $processInfo.UseShellExecute = $false

        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = $processInfo
        $process.Start() | Out-Null
        $output = $process.StandardOutput.ReadToEnd()
        $error = $process.StandardError.ReadToEnd()
        $process.WaitForExit()

        if ($process.ExitCode -eq 0) {

            Log-Message "Registered DLL: $dllPath", $logFilePath
            Write-Output "Registered DLL: $dllPath"
            
            Log-Message "", $logFilePath
            Write-Output ""

            Log-Message "Output: $output", $logFilePath
            #Write-Output "Output: $output"

        } else {

            Log-Message "Error registering DLL: $dllPath - $error", $logFilePath
            Write-Error "Error registering DLL: $dllPath - $error"
        }

    } catch {

        Log-Message "Error registering DLL: $dllPath - $_", $logFilePath
        Write-Error "Error registering DLL: $dllPath - $_"

    }
}

# unregister the DLL using regasm.exe
function Unregister-Dll {
    param (
        [string]$dllPath
    )

    try {

        Log-Message "Attempting to unregister DLL: $dllPath", $logFilePath
        Write-Output "Attempting to unregister DLL: $dllPath"

        $processInfo = New-Object System.Diagnostics.ProcessStartInfo

        if (Test-Path $regasmPath64) {

            $processInfo.FileName = $regasmPath64
            Log-Message "Using 64-bit regasm.exe", $logFilePath

        } elseif (Test-Path $regasmPath32) {

            $processInfo.FileName = $regasmPath32
            Log-Message "Using 32-bit regasm.exe", $logFilePath

        } else {
            Log-Message "Regasm.exe not found. Is .Net framework 4.8 installed?", $logFilePath
            throw "Regasm.exe not found. Is .Net framework 4.8 installed?"
        }
        
        $processInfo.Arguments = "/unregister $dllPath"
        $processInfo.RedirectStandardOutput = $true
        $processInfo.RedirectStandardError = $true
        $processInfo.UseShellExecute = $false

        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = $processInfo
        $process.Start() | Out-Null
        $output = $process.StandardOutput.ReadToEnd()
        $error = $process.StandardError.ReadToEnd()
        $process.WaitForExit()

        if ($process.ExitCode -eq 0) {
            Log-Message "Unregistered DLL: $dllPath", $logFilePath
            Write-Output "Unregistered DLL: $dllPath"
            
            Log-Message "", $logFilePath
            Write-Output ""

            Log-Message "Output: $output", $logFilePath
            #Write-Output "Output: $output"

        } else {

            Log-Message "Error unregistering DLL: $dllPath - $error", $logFilePath
            Write-Error "Error unregistering DLL: $dllPath - $error"
        }

    } catch {

        Log-Message "Error unregistering DLL: $dllPath - $_", $logFilePath
        Write-Error "Error unregistering DLL: $dllPath - $_"
    }
}

# verify whether the dll registration went through successfully
function Check-DllRegistration {
    param (
        [string]$dllPath
    )
    $dllName = [System.IO.Path]::GetFileNameWithoutExtension($dllPath)
    $guid = "{8E5CD5CA-64E0-479A-B62F-B1FC00FF0227}".ToLower()
    $keyPath = "Registry::HKEY_CLASSES_ROOT\CLSID\$guid"

    Log-Message "Checking registry path: $keyPath", $logFilePath
    Write-Output "Checking registry path: $keyPath"

    try {

        if (Test-Path $keyPath) {

            $key = Get-ItemProperty -Path $keyPath
            $message = "DLL is registered: $dllPath"
            Log-Message $message, $logFilePath

            Log-Message "", $logFilePath
            Write-Output ""

            return $message

        } else {

            $message = "DLL is not registered: $dllPath"
            Log-Message $message, $logFilePath
            
            Log-Message "", $logFilePath
            Write-Output ""

            return $message

        }

    } catch {

        $message = "DLL is not registered: $dllPath"
        Log-Message $message, $logFilePath
        
        Log-Message "", $logFilePath
        Write-Output ""

        return $message
    }
}

# ====== Registry Functions =======

# full list of Calibre-supported extensions from the registry
function GetCalibreExtensions {

    $calibreAssocKey = "HKCU:\SOFTWARE\calibre\calibre64bit\Capabilities\FileAssociations"
    if (Test-Path $calibreAssocKey) {

        $calibreProperties = Get-ItemProperty -Path $calibreAssocKey | Get-Member -MemberType NoteProperty

        Log-Message "Calibre File associations retrieved.", $logFilePath
        Write-Output "Calibre File associations retrieved." -ForegroundColor Green

        return $calibreProperties.Name | ForEach-Object { "$($_)" }

    } else {

        Log-Message "Calibre file association registry key not found. Using default extensions.", $logFilePath
        Write-Output "Calibre file association registry key not found. Using default extensions." -ForegroundColor Yellow

        return @(".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu", ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html", ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar")
    }
}

<# adding registry entries for all Calibre-supported extensions
this should be redundant, since the C# code already takes care of this upon registration #>
function Add-RegistryEntries {

    $extensions = GetCalibreExtensions
    $guid = "8E5CD5CA-64E0-479A-B62F-B1FC00FF0227".ToLower()

    foreach ($ext in $extensions) {
        Add-RegistryEntry -extension $ext -guid $guid
    }

    Log-Message "Added $assemblyName registry entries for all Calibre-supported extensions", $logFilePath
    Write-Output "Added $assemblyName registry entries for all Calibre-supported extensions"
}

function Add-RegistryEntry {
    param (
        [string]$extension,
        [string]$guid
    )

    $keyPath = "HKCU:\SOFTWARE\Classes\SystemFileAssociations\$extension\Shellex\ContextMenuHandlers\$assemblyName"
    if (-not (Test-Path $keyPath)) {

        New-Item -Path $keyPath -Force | Out-Null
    }

    Set-ItemProperty -Path $keyPath -Name "(default)" -Value "{$guid}"
    # Log-Message "Added registry entry for extension: $extension"
}

<# removing registry entries for all Calibre-supported extensions
this should be redundant, since the C# code already takes care of this upon unregistration #>
function Remove-RegistryEntries {

    $extensions = GetCalibreExtensions

    foreach ($ext in $extensions) {
        Remove-RegistryEntry -extension $ext
    }

    Log-Message "Removed $assemblyName registry entries for all Calibre-supported extensions", $logFilePath
    Write-Output "Removed $assemblyName registry entries for all Calibre-supported extensions"
}

function Remove-RegistryEntry {
    param (
        [string]$extension
    )
    $keyPath = "HKCU:\SOFTWARE\Classes\SystemFileAssociations\$extension\Shellex\ContextMenuHandlers\$assemblyName"

    if (Test-Path $keyPath) {

        Remove-Item -Path $keyPath -Recurse -Force
        # Log-Message "Removed registry entry for extension: $extension", $logFilePath
    }
}

# ====== RUNTIME =======

# Ensure the log file is created if logging is enabled
if ($EnableLogging) {

    New-Item -ItemType File -Path $logFilePath -Force | Out-Null
}

Log-Message "===== Setup script started =====", $logFilePath
Write-Output "===== Setup script started ====="
            
Log-Message "", $logFilePath
Write-Output ""

# gather DLL files in the script directory
if ($isRepacked){

    $dllFiles = Get-ChildItem -Path $PSScriptRoot -Filter "$assemblyName.dll"

} else {

    $dllFiles = Get-ChildItem -Path $PSScriptRoot -Filter *.dll | Where-Object { $_.Name -in "SharpShell.dll", "$assemblyName.dll" }
}

# if no DLL files, bye
if (-not $dllFiles) {

    Log-Message "No DLLs found in the script directory. Exiting.", $logFilePath
    throw "No DLLs found in the script directory."
}

# if no parameters, or only -i, Install
if ((-not $PSBoundParameters.Count) -or ($i -and (-not $u -and -not $re))) {

    Log-Message "Proceeding with Installation", $logFilePath
    Write-Output "Proceeding with Installation"

    foreach ($dllFile in $dllFiles) {

        $dllPath = $dllFile.FullName
        Register-Dll -dllPath $dllPath
        $registrationMessage = Check-DllRegistration -dllPath $dllPath
        Write-Output $registrationMessage
    }

    # Add-RegistryEntries
} 

# if only -u, Uninstall
elseif ($u -or (-not $i -and -not $re)) {

    Log-Message "Proceeding with Uninstallation", $logFilePath
    Write-Output "Proceeding with Uninstallation"

    foreach ($dllFile in $dllFiles) {

        $dllPath = $dllFile.FullName
        Unregister-Dll -dllPath $dllPath
        $registrationMessage = Check-DllRegistration -dllPath $dllPath
        Write-Output $registrationMessage
    }

    Remove-RegistryEntries
}

# if -re, Restart Explorer
if ($re) {

	# Stop all explorer instances
	Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue

	# Delay to ensure it's fully closed
	Start-Sleep -Seconds 3  
		
	$explorer = Get-Process | Where-Object { $_.ProcessName -eq "explorer" } -ErrorAction SilentlyContinue

	# Start if it hasn't already started (avoids new window)
	if (-Not ($explorer)) {Start-Process "explorer.exe" -ErrorAction SilentlyContinue}

    Log-Message "Windows Explorer restarted.", $logFilePath
    Write-Output "Windows Explorer restarted."


} else {

    $response = Read-Host "Do you want to restart Explorer now? (y/n)"
    if ($response -eq "y") {
       
		# Stop all explorer instances
		Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue

		# Delay to ensure it's fully closed
		Start-Sleep -Seconds 3  
		
		$explorer = Get-Process | Where-Object { $_.ProcessName -eq "explorer" } -ErrorAction SilentlyContinue

		# Start if it hasn't already started (avoids new window)
		if (-Not ($explorer)) {Start-Process "explorer.exe" -ErrorAction SilentlyContinue}

        Log-Message "Windows Explorer restarted.", $logFilePath
        Write-Output "Windows Explorer restarted."

    }
}

Log-Message "Setup script completed.", $logFilePath
Write-Output "Setup script completed."