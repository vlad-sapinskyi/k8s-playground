import * as const from './constants.bicep'
import { environmentType, locationType, resourceType } from './types.bicep'

@export()
func getResourceName(res resourceType, env environmentType, location locationType, prefix string?, postfix string?) string =>
  join(filter([
    getResourceShortName(res)!
    prefix!
    const.appName
    env
    getLocationShortName(location)! 
    postfix!
  ], value => value != null), '-')

@export()
func getModuleFullName(name string, env environmentType, location locationType, postfix string?) string =>
  join(filter([
    const.appName
    'module'
    replace(name, '-', '')
    env
    getLocationShortName(location)!
    postfix!
  ], value => value != null), '-')

func getLocationShortName(location locationType) string? =>
    (location == 'westeurope') ? 'we'
  : (location == 'swedencentral') ? 'sdc'
  : null

func getResourceShortName(res resourceType) string? =>
    (res == 'ResourceGroup') ? 'rg'
  : (res == 'VirtualNetwork') ? 'vnet'
  : (res == 'Subnet') ? 'snet'
  : (res == 'PublicIpAddress') ? 'pip'
  : (res == 'VirtualMachine') ? 'vm'
  : (res == 'Disk') ? 'disk'
  : (res == 'NetworkInterface') ? 'nic'
  : (res == 'NetworkSecurityGroup') ? 'nsg'
  : null
