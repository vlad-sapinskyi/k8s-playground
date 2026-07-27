import * as const from 'constants.bicep'

using 'main.bicep'

param env = 'dev'
param location = 'swedencentral'

param network = {
  vnetAddressPrefix: '10.1.0.0/24'
  subnetAddressPrefix: '10.1.0.0/28'
}

param instances = [
  {
    name: 'control-plane'
    privateIp: '10.1.0.5'
    usePublicIp: true
  }
  {
    name: 'worker-01'
    privateIp: '10.1.0.6'
  }
]

param hostConfig = {
  sku: 'Standard_B4als_v2'
  diskSize: 40
  image: {
    publisher: 'canonical'
    offer: 'ubuntu-24_04-lts'
    sku: 'server'
    version: 'latest'
  }
  ports: [
    {
      number: '22'
      priority: 1100
    }
  ]
  admin: {
    userName: const.appName
    publicKey: loadTextContent('../ssh/ssh-dev.pub')
  }
}
