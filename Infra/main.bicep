param branchName string
param location string = resourceGroup().location
param repositoryUrl string
param accountManagerDaprBaseUrl string
param signalRDaprNegotiateUrl string
param accountManagerFunctionBaseUrl string
param accountManagerFunctionKey string
param signalRFunctionNegotiateUrl string
param signalRFunctionKey string

var branch = toLower(last(split(branchName, '/')))

param sku string = 'F1'
var appServicePlanName = '${branch}-bmu-hostingplan'
var webSiteName = 'bank-management-ui'
var linuxFxVersion = 'DOTNETCORE|Latest'


resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}
resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: webSiteName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      appSettings: [
       
        {
            name: 'ASPNETCORE_URLS'
            value: 'http://+:80'
        }
        {
            name: 'BMSD_ACCOUNT_MANAGER_URL'
            value: accountManagerDaprBaseUrl
        }
        {
            name: 'BMSD_SIGNALR_URL'
            value: signalRDaprNegotiateUrl
        }
        {
            name: 'BMS_ACCOUNT_MANAGER_URL'
            value: accountManagerFunctionBaseUrl
        }
        {
            name: 'BMS_ACCOUNT_MANAGER_KEY'
            value: accountManagerFunctionKey
        }
        {
            name: 'BMS_SIGNALR_URL'
            value: signalRFunctionNegotiateUrl
        }
        {
            name: 'BMS_SIGNALR_NEGOTIATE_KEY'
            value: signalRFunctionKey
        }
      ]
    }
  }
}

resource srcControls 'Microsoft.Web/sites/sourcecontrols@2022-03-01' = {
  name: '${appService.name}/web'
  properties: {
    repoUrl: repositoryUrl
    branch: branch
    isManualIntegration: true
  }
}