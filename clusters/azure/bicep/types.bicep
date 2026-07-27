@export()
type environmentType =
  | 'prod'
  | 'test'
  | 'dev'

@export()
type locationType =
  | 'westeurope'
  | 'swedencentral'

@export()
type resourceType =
  | 'ResourceGroup'
  | 'VirtualNetwork'
  | 'Subnet'
  | 'PublicIpAddress'
  | 'VirtualMachine'
  | 'Disk'
  | 'NetworkInterface'
  | 'NetworkSecurityGroup'

@export()
type networkType = {
  vnetAddressPrefix: string
  subnetAddressPrefix: string
}

@export()
type instanceType = {
  name: string
  privateIp: string
  usePublicIp: bool?
}

@export()
type imageType = {
  publisher: string
  offer: string
  sku: string
  version: string
}

@export()
type portType = {
  number: string
  priority: int
}

@export()
type adminType = {
  userName: string
  @secure()
  publicKey: string
}

@export()
type hostConfigType = {
  sku: string
  diskSize: int
  image: imageType
  ports: portType[]
  admin: adminType
}
