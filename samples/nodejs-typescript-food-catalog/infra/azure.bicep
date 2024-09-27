param resourceBaseName string
param location string = resourceGroup().location

param appClientId string
@secure()
param appClientSecret string
param appTenantId string

// create storage account to store table data
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: resourceBaseName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

// create app service plan for function app
resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: resourceBaseName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

// create function app
resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: resourceBaseName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}


// create azure key vault
resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: resourceBaseName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: functionApp.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
    ]
  }
}

// add client secret to key vault
resource appClientSecretVault 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'clientSecret'
  properties: {
    value: appClientSecret
  }
}

// add storage account connection string to key vault
resource storageAccountConnectionStringVault 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'storageAccountConnectionString'
  properties: {
    value: storageAccountConnectionString
  }
}

// set app settings on the function app
resource siteConfig 'Microsoft.Web/sites/config@2021-02-01' = {
  name: 'appsettings'
  parent: functionApp
    properties: {
      AzureWebJobsStorage: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=storageAccountConnectionString)'
      WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=storageAccountConnectionString)'
      WEBSITE_CONTENTSHARE: toLower(resourceBaseName)
      FUNCTIONS_EXTENSION_VERSION: '~4'
      WEBSITE_NODE_DEFAULT_VERSION: '~18'
      APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsights.properties.InstrumentationKey
      FUNCTIONS_WORKER_RUNTIME: 'node'
      WEBSITE_RUN_FROM_PACKAGE: '1'
      ENTRA_APP_CLIENT_ID: appClientId
      ENTRA_APP_CLIENT_SECRET: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=clientSecret)'
      ENTRA_APP_TENANT_ID: appTenantId
      NOTIFICATION_ENDPOINT: notificationEndpoint
      GRAPH_SCHEMA_STATUS_INTERVAL: '10'
    }
}

// create application insights resource
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: resourceBaseName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
var notificationEndpoint = 'https://${functionApp.properties.defaultHostName}'

// output values to env.dev so they can be used by other actions
output NOTIFICATION_FUNCTION_RESOURCE_ID string = functionApp.id
output SECRET_STORAGE_ACCOUNT_CONNECTION_STRING string = storageAccountConnectionString
output NOTIFICATION_ENDPOINT string = notificationEndpoint
output NOTIFICATION_DOMAIN string = functionApp.properties.defaultHostName
