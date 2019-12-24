[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$RG="begim-storsdkfunc-demo-rg",

  [Parameter(Mandatory=$False)]
  [string]$ResourcesPrefix="begim1975"
)

#az login
$Location="westeurope"

# create RG
$rgExists = az group exists -n $RG
if ( $rgExists -eq $False ){
    Write-Output "Creating RG"
  az group create -n $RG -l $Location
}

$templateFile = "deploy.json"

az group deployment create -n "demostorsdkfunc" -g $RG --template-file "$templateFile" --parameters resourcesPrefix=$ResourcesPrefix 

Write-Host "deployed resources, pushing function code"

## function 
$funcname = "$ResourcesPrefix" + "-demostorsdkfunc"
dotnet publish "..\HttpTriggerCSharp\storagesdkfunc.csproj"
$compress = @{
  Path= "..\HttpTriggerCSharp\bin\Debug\netcoreapp2.1\publish\*"
  CompressionLevel = "Fastest"
  DestinationPath = "HttpTriggerCSharp.zip"
}
Compress-Archive @compress -Force
az functionapp deployment source config-zip  -g $RG -n $funcname --src "HttpTriggerCSharp.zip"

write-host "published function" -ForegroundColor Green

az functionapp identity assign --name $funcname --resource-group $RG
write-host "set sys identity (MSI) for func app"
write-host "create container and assign storage IAM permissions"


