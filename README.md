# k8s-playground

Kubernetes GitOps playground: Argo CD, Istio, Kargo

## Overview

- **[Prerequisites](#prerequisites)**
- **[SSH Key Setup](#ssh-key-setup)**
- **[Deploy Infrastructure](#deploy-infrastructure)**
- **[SSH Access](#ssh-access)**
- **[Prerequisites](#prerequisites)**


## Prerequisites

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- PowerShell 7+
- OpenSSH (built into Windows 10/11)


## SSH Key Setup

Generate an SSH key pair locally (one-time setup):

```powershell
ssh-keygen -t ed25519 -C 'k8s-playground' -N '' -f '.\clusters\azure\ssh\k8s-playground'
```

This creates two files:
- `clusters/azure/ssh/k8s-playground` - private key (never share or commit this)
- `clusters/azure/ssh/k8s-playground.pub` - public key (injected into VMs during provisioning)


## Deploy Infrastructure

### Login to Azure:

```powershell
az login
```

### Run deployment script:

```powershell
.\clusters\azure\Deploy-Infrastructure.ps1 -Environment [Dev/Test/Prod] -Location [SwedenCentral/WestEurope]
```

The script deploys a VNet, control plane VM (with public IP) and worker VMs using the public SSH key automatically.


## SSH Access

> **Note:** Control plane and worker private IPs are defined in `main-[dev/test/prod].bicepparam` files.

### Connect to control plane

```powershell
ssh -i <path-to-public-ssh-key> k8s-playground@<control-plane-public-ip>
```

### Connect to worker (via control plane)

If you prefer to SSH from the control plane to workers directly, copy the private key to the CP:

```powershell
scp -i <path-to-public-ssh-key> <path-to-public-ssh-key> k8s-playground@<cp-public-ip>:~/.ssh/id_ed25519
```

Then on the control plane, fix permissions:

```bash
chmod 600 ~/.ssh/id_ed25519
```

And connect to desired worker:

```bash
ssh k8s-playground@<worker-private-ip>
```
