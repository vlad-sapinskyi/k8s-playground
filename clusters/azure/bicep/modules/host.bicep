import { environmentType, locationType, instanceType, hostConfigType } from '../types.bicep'
import { getResourceName } from '../functions.bicep'

param env environmentType
param location locationType
param instance instanceType
param subnetId string
param hostConfig hostConfigType

var pipName = getResourceName('PublicIpAddress', env, location, null, instance.name)
var nsgName = getResourceName('NetworkSecurityGroup', env, location, null, instance.name)
var nicName = getResourceName('NetworkInterface', env, location, null, instance.name)
var vmName = getResourceName('VirtualMachine', env, location, null, instance.name)
var diskName = getResourceName('Disk', env, location, null, instance.name)

// Create Public IP (if required)

resource pip 'Microsoft.Network/publicIPAddresses@2025-07-01' = if (instance.?usePublicIp == true) {
  name: pipName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Regional'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: 'Static'
  }
}

// Create Network Security Group

resource nsg 'Microsoft.Network/networkSecurityGroups@2025-07-01' = {
  name: nsgName
  location: location
  properties: {
    securityRules: [
      for port in hostConfig.ports: {
        name: 'AllowInBoundPort${port.number}'
        properties: {
          priority: port.priority
          protocol: 'Tcp'
          access: 'Allow'
          direction: 'Inbound'
          sourceAddressPrefix: '*'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: port.number
        }
      }
    ]
  }
}

// Create Network Interface

resource nic 'Microsoft.Network/networkInterfaces@2025-07-01' = {
  name: nicName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ip-config'
        properties: {
          subnet: {
            id: subnetId
          }
          privateIPAllocationMethod: 'Static'
          privateIPAddressVersion: 'IPv4'
          privateIPAddress: instance.privateIp
          publicIPAddress: instance.?usePublicIp == true ? { id: pip.id } : null
        }
      }
    ]
    networkSecurityGroup: {
      id: nsg.id
    }
  }
}

// Create Virtual Machine

resource vm 'Microsoft.Compute/virtualMachines@2026-03-01' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: hostConfig.sku
    }
    storageProfile: {
      osDisk: {
        osType: 'Linux'
        name: diskName
        diskSizeGB: hostConfig.diskSize
        createOption: 'FromImage'
        deleteOption: 'Delete'
      }
      imageReference: hostConfig.image
    }
    osProfile: {
      computerName: instance.name
      adminUsername: hostConfig.admin.userName
      linuxConfiguration: {
        disablePasswordAuthentication: true
        ssh: {
          publicKeys: [
            {
              path: '/home/${hostConfig.admin.userName}/.ssh/authorized_keys'
              keyData: hostConfig.admin.publicKey
            }
          ]
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic.id
          properties: {
            deleteOption: 'Delete'
          }
        }
      ]
    }
  }
}
