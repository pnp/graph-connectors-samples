param functionAppName string
param appServicePlanName string
param storageAccountName string
param documentsApiUrl string
param documentsApiScopes string
param entraTenantId string
param entraClientId string

@secure()
param entraClientSecret string

param location string = resourceGroup().location
param graphSchemaStatusInterval string = '60'
param functionsWorkerRuntime string = 'dotnet-isolated'
param functionsExtensionVersion string = '~4'

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource functionApp 'Microsoft.Web/sites@2021-01-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionsWorkerRuntime
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: functionsExtensionVersion
        }
        {
          name: 'GRAPH_SCHEMA_STATUS_INTERVAL'
          value: graphSchemaStatusInterval
        }
        {
          name: 'DocumentsApi:Url'
          value: documentsApiUrl
        }
        {
          name: 'DocumentsApi:Scopes'
          value: documentsApiScopes
        }
        {
          name: 'Entra:TenantId'
          value: entraTenantId
        }
        {
          name: 'Entra:ClientId'
          value: entraClientId
        }
        {
          name: 'entraClientSecret'
          value: entraClientSecret
        }
      ]
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

output function_app_id string = functionApp.id
output bot_endpoint string = 'https://${functionApp.properties.defaultHostName}'
