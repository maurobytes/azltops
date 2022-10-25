@description('Resource Location')
param location string = resourceGroup().location
@description('Cosmos DB account name')
param cosmosdbAccountName string = 'cosmos-${uniqueString(resourceGroup().id)}'
@description('The name for the SQL API database')
param databaseName string = 'CostumesDB'
@description('The name for the SQL API container')
param containerName string = 'Costumes'

@description('The name of the function app that you wish to create.')
param appName string = uniqueString(resourceGroup().id)

@description('Storage Account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
])
param storageAccountType string = 'Standard_LRS'
@description('The SKU of App Service Plan.')
param sku string = 'B1'
@description('The language worker runtime to load in the function app.')
@allowed([
  'node'
  'dotnet'
  'java'
])
param runtime string = 'dotnet'

@description('The Runtime stack of current web app')
param linuxFxVersion string = 'NODE|18-lts'

var functionAppName = 'fnapp-${appName}'
var nodeWebAppName = 'node-${appName}'
var hostingPlanName = 'asp-${appName}'
var workspaceName = 'log-${appName}'
var applicationInsightsName = 'ai-${appName}'
var storageAccountName = '${uniqueString(resourceGroup().id)}azfunctions'
var functionWorkerRuntime = runtime
var loadTestName = 'loadtest-${appName}'

@description('This is the built-in DocumentDB Account Contributor role. See https://docs.microsoft.com/azure/role-based-access-control/built-in-roles#documentdb-account-contributor')
var cosmosDBContributorRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5bd9cd88-fe45-4216-938b-f97437e15450')

resource cosmosdbAccount 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: toLower(cosmosdbAccountName)
  location: location
  properties: {
    enableFreeTier: false
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-05-15' = {
  parent: cosmosdbAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 1000
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-05-15' = {
  parent: database
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [ 
          '/id' 
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
      }
    }
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'Storage'
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: workspaceName
  location: location
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: sku
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource nodeWebApp 'Microsoft.Web/sites@2021-03-01' = {
  name: nodeWebAppName
  location: location
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      appCommandLine: 'pm2 serve /home/site/wwwroot --no-daemon --spa'
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      appSettings: [
        {
          name: 'APPINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
            name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
            value: '~3'
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
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
      netFrameworkVersion: 'v6.0'
      linuxFxVersion: 'DOTNET|6.0'
      cors:{
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
            name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
            value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionWorkerRuntime
        }
        {
          name: 'CosmosDbConnectionString'
          value: cosmosdbAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
      ]
    }
    httpsOnly: true
  }
}

resource cosmosDBFunctionAppPermissions 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(cosmosdbAccount.id, functionApp.name, cosmosDBContributorRole)
  scope: cosmosdbAccount
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: cosmosDBContributorRole
  }
}

resource azureLoadTestService 'Microsoft.LoadTestService/loadtests@2022-04-15-preview' = {
  name: loadTestName
  location: location
}

output functionAppName string = functionApp.name
output functionAppUrl string = functionApp.properties.defaultHostName
output nodeWebAppName string = nodeWebApp.name
output nodeWebAppUrl string = nodeWebApp.properties.defaultHostName
output loadTestName string = azureLoadTestService.name
