#Requires -RunAsAdministrator

[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [ValidateSet('Install', 'Update', 'Uninstall')]
    [string] $Action,

    [ValidateNotNullOrEmpty()]
    [string] $Distro = 'Ubuntu'
)

$ErrorActionPreference = 'Stop'

Write-Verbose "Checking WSL installation status..."
wsl --status 2>&1 | Out-Null
$wslInstalled = ($LASTEXITCODE -eq 0)

switch ($Action) {
    'Install' {
        if (-not $wslInstalled) {
            Write-Host "Installing..."
            wsl --install -d $Distro
            exit 0
        }
    }
    'Update' {
        if ($wslInstalled) {
            Write-Host "Updating..."
            wsl --update
            exit 0
        }
    }
    'Uninstall' {
        if ($wslInstalled) {
            Write-Host "Uninstalling..."
            wsl --unregister $Distro 
            wsl --uninstall
            exit 0
        }
    }
}

if (-not $wslInstalled) {
    Write-Host "WSL is not installed:"
    Write-Host "    Re-run this script with '-Action Install' parameter."
} else {
    Write-Host "WSL is already installed:"
    $versionInfo = wsl --version
    Write-Host "    $($versionInfo[0])"
}
