param appName string = 'listjj-blazorfront'
param location string = resourceGroup().location

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
          name: 'ApiEndpoint'
          value: 'https://listjj-api.azurewebsites.net'
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
      linuxFxVersion: 'DOCKER|jarekj9/listjj_frontend_blazor:latest'
    }
    serverFarmId: appServicePlan.id
  }
  identity: {
    type: 'SystemAssigned'
  }
}
