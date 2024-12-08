param appName string = 'listjj-api'
param location string = resourceGroup().location

param mssqlConnStringSecret string
param googleAuthClientSecret string

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
          value: mssqlConnStringSecret
        }

        {
          name: 'Authentication__Google__ClientSecret'
          value: googleAuthClientSecret
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
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
