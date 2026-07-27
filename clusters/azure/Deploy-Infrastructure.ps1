[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('Dev', 'Test', 'Prod')]
    [string] $Environment,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('SwedenCentral', 'WestEurope')]
    [string] $Location
)
process {
    $ErrorActionPreference = 'Stop'

    Write-Host "Environment:    $Environment" -ForegroundColor Blue
    Write-Host "Location:       $Location" -ForegroundColor Blue

    # Set application name
    $appName = 'k8s-playground'

    # Set environment name
    $envName = $Environment.ToLower()

    # Set deployment location name
    $locationName = $Location.ToLower()
    $locationShortName = 'sdc'
    if ('WestEurope' -eq $Location) {
        $locationShortName = 'we'
    }

    # Set subscription ID to 'Visual Studio Subscription'
    $subscriptionId = 'd8e9de08-b908-4b06-bbcd-aec30974acf3'

    try {
        # Set Azure context
        Write-Host "`nSetting Azure context to '$subscriptionId' subscription...`n" -ForegroundColor Green
        az account set --subscription $subscriptionId
        $account = az account show | ConvertFrom-Json
        Write-Host ($account | Format-List | Out-String)

        # Deploy infrastructure
        $deploymentName = "$appName-$envName-$locationShortName"
        $deploymentTemplateFile = "$PSScriptRoot\bicep\main.bicep"
        $deploymentParameters = "$PSScriptRoot\bicep\main-$envName.bicepparam"
        Write-Host "`nDeploying infrastructure ...`n" -ForegroundColor Green
        az deployment sub create --name $deploymentName --location $locationName --template-file $deploymentTemplateFile --parameters $deploymentParameters

        Write-Host "`nDone!`n" -ForegroundColor Green
    }
    catch {
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host $_.ScriptStackTrace
        exit 1
    }
}
