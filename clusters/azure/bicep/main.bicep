targetScope = 'subscription'

import { environmentType, locationType, networkType, instanceType, hostConfigType } from 'types.bicep'
import { getResourceName, getModuleFullName } from 'functions.bicep'

param env environmentType
param location locationType
param network networkType
param instances instanceType[]
param hostConfig hostConfigType

var rgName = getResourceName('ResourceGroup', env, location, null, null)

// Create Resource Group

resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: rgName
  location: location
}

// Run 'network' module

module networkModule 'modules/network.bicep' = {
  name: getModuleFullName('network', env, location, null)
  scope: rg
  params: {
    env: env
    location: location
    network: network
  }
}

// Run 'Host' modules

module hostModules 'modules/host.bicep' = [
  for instance in instances: {
    name: getModuleFullName('host', env, location, instance.name)
    scope: rg
    params: {
      env: env
      location: location
      instance: instance
      subnetId: networkModule.outputs.subnetId
      hostConfig: hostConfig
    }
  }
]
