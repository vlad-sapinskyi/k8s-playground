import { environmentType, locationType, networkType } from '../types.bicep'
import { getResourceName } from '../functions.bicep'

param env environmentType
param location locationType
param network networkType

var vnetName = getResourceName('VirtualNetwork', env, location, null, null)
var snetName = getResourceName('Subnet', env, location, null, null)

// Create Virtual Network

resource vnet 'Microsoft.Network/virtualNetworks@2025-07-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        network.vnetAddressPrefix
      ]
    }
  }

  // Create Subnet

  resource subnet 'subnets' = {
    name: snetName
    properties: {
      addressPrefix: network.subnetAddressPrefix
    }
  }
}

output subnetId string = vnet::subnet.id
