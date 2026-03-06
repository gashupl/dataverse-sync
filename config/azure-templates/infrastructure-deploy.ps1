# Deploy Azure Function App using Bicep template
# Simple deployment script for DataverseSync Function App

param(
    [Parameter(Mandatory = $true)]
    [string]$Environment,
    
    [Parameter(Mandatory = $false)]
    [string]$SubscriptionId
)

# Set variables based on environment
$ResourceGroupName = "rg-dvsync-$Environment"
$Location = "West Europe"  # Changed to support Flex Consumption

# Set script location and template paths
$ScriptRoot = $PSScriptRoot
$TemplateFile = Join-Path $ScriptRoot "template.bicep"
$ParametersFile = Join-Path $ScriptRoot "template.bicepparam"

# Set subscription if provided
if ($SubscriptionId) {
    az account set --subscription $SubscriptionId
}

# Create resource group if it doesn't exist
$rgExists = az group exists --name $ResourceGroupName
if ($rgExists -eq "false") {
    Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Yellow
    az group create --name $ResourceGroupName --location $Location
} else {
    Write-Host "Resource group already exists: $ResourceGroupName" -ForegroundColor Green
}

# Deploy the template
$deploymentName = "dataversesync-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

az deployment group create `
    --resource-group $ResourceGroupName `
    --name $deploymentName `
    --template-file $TemplateFile `
    --parameters $ParametersFile `
    --parameters environmentSuffix=$Environment

Write-Host "Deployment complete!" -ForegroundColor Green