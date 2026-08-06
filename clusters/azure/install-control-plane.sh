#!/usr/bin/env bash
set -euo pipefail

WORKER_NAME="${1:-worker-01}"

echo "=== Step 1: Install k8s snap ==="
if snap list k8s &>/dev/null; then
    echo "k8s snap already installed."
else
    sudo snap install k8s --classic --channel=1.35-classic/stable
fi

echo "=== Step 2: Bootstrap cluster ==="
if sudo k8s status &>/dev/null; then
    echo "Cluster already bootstrapped."
else
    sudo k8s bootstrap
fi

echo "=== Step 3: Wait for cluster ready ==="
sudo k8s status --wait-ready

echo "=== Step 4: Enable ingress ==="
if sudo k8s status | grep -q "ingress:.*enabled"; then
    echo "Ingress already enabled."
else
    sudo k8s enable ingress
fi

echo "=== Step 5: Setup kubectl alias ==="
mkdir -p ~/.kube
sudo k8s kubectl completion bash > ~/.kube/k8s-completion.bash

if ! grep -q "alias k=" ~/.bash_aliases 2>/dev/null; then
    cat >> ~/.bash_aliases << 'EOF'
alias k='sudo k8s kubectl'
[ -f ~/.kube/k8s-completion.bash ] && source ~/.kube/k8s-completion.bash
complete -o default -F __start_kubectl k
EOF
fi
source ~/.bash_aliases

echo "=== Step 5: Generate join token for ${WORKER_NAME} ==="
sudo k8s get-join-token "${WORKER_NAME}" --worker
