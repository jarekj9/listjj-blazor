@description('Appname')
param appName string = 'listjj-api'

@description('Use the Resource Group Location')
param location string = resourceGroup().location

resource mssqlConnStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'listjj-keyvault/mssqlConnString'
  properties: {
      contentType: "string"
      value: "string"
  }
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
          value: mssqlConnStringSecret.properties.value
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
