param branchName string
param location string = resourceGroup().location
param repositoryUrl string
param accountManagerBaseUrl string
param accountManagerKey string
param signalRNegotiateUrl string
param signalRKey string

param tags object = {}
var branch = toLower(last(split(branchName, '/')))

param sku string = 'F1'
var storageAccountName = '${branch}bmustorageaccount'
var appServicePlanName = '${branch}-bmu-hostingplan'
var WebAppName = '${branch}-bank-management-ui' 
var webSiteName = toLower('wapp-bank-management-ui')
var linuxFxVersion="DOTNETCORE|Latest"


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