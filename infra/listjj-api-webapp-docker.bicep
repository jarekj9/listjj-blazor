@description('Appname')
param appName string = 'listjj-api'

@description('Use the Resource Group Location')
param location string = resourceGroup().location

resource kv 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'listjj-keyvault'
  scope: resourceGroup()
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'service-plan-listjj'
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: 'F1'
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appName
  location: location
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'ConnectionStrings__MsSqlDbContext'
          value: reference(kv.id, '2021-06-01', 'Full').secrets.mssqlConnStringSecret
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Docker'
        }
        {
          name: 'ASPNETCORE_HTTP_PORTS'
          value: '80'
        }
      ]
      linuxFxVersion: 'DOCKER|jarekj9/listjj_blazor_api:latest'
    }
    serverFarmId: appServicePlan.id
  }
  identity: {
    type: 'SystemAssigned'
  }
}
