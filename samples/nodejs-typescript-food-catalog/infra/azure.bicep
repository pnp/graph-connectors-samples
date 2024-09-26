param resourceBaseName string
param location string = resourceGroup().location

param appClientId string
@secure()
param appClientSecret string
param appTenantId string

// create storage account to store table data
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: resourceBaseName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// create app service plan for function app
resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: resourceBaseName
  location: location
  sku: {
    name: 'Y1'
  }
  properties: {}
}

// create a Function app to host the notification API
resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: resourceBaseName
  kind: 'functionapp'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      ftpsState: 'FtpsOnly'
    }
  }
}

// set app settings on the Function app
resource siteConfig 'Microsoft.Web/sites/config@2021-02-01' = {
  name: 'appsettings'
  parent: functionApp
    properties: {
      FUNCTIONS_EXTENSION_VERSION: '~4'
      FUNCTIONS_WORKER_RUNTIME: 'node'
      WEBSITE_RUN_FROM_PACKAGE: '1'
      WEBSITE_NODE_DEFAULT_VERSION: '~18'
      AzureWebJobsStorage: storageAccountConnectionString
      AAD_APP_CLIENT_ID: appClientId
      AAD_APP_CLIENT_SECRET: appClientSecret
      AAD_APP_TENANT_ID: appTenantId
      NOTIFICATION_ENDPOINT: notificationEndpoint
      GRAPH_SCHEMA_STATUS_INTERVAL: '10'
    }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
var notificationEndpoint = 'https://${functionApp.properties.defaultHostName}'

// output values to env.dev so they can be used by other actions
output NOTIFICATION_FUNCTION_RESOURCE_ID string = functionApp.id
output SECRET_STORAGE_ACCOUNT_CONNECTION_STRING string = storageAccountConnectionString
output NOTIFICATION_ENDPOINT string = notificationEndpoint
