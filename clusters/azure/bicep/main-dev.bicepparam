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
  sku: 'Standard_D2as_v5'
  diskSize: 64
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
    publicKey: loadTextContent('../ssh/k8s-playground.pub')
  }
}
