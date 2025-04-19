param appName string = 'listjj-api'
param location string = resourceGroup().location

param mssqlConnStringSecret string
//param mysqlConnStringSecret string
param googleAuthClientSecret string
param microsoftTenantId string
param microsoftClientId string

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
          //name: 'ConnectionStrings__MySqlDbContext'
          //value: mysqlConnStringSecret
        }

        {
          name: 'Authentication__GoogleAuthOptions__ClientId'
          value: '1034243776717-9qsk2ud7ltr621vf14dph6o0nuqbd4bq.apps.googleusercontent.com'
        }
        {
          name: 'Authentication__GoogleAuthOptions__ClientSecret'
          value: googleAuthClientSecret
        }
        {
          name: 'Authentication__MicrosoftAuthOptions__TenantId'
          value: microsoftTenantId
        }
        {
          name: 'Authentication__MicrosoftAuthOptions__ClientId'
          value: microsoftClientId
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
