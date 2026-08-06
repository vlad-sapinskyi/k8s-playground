#!/usr/bin/env bash
set -euo pipefail

JOIN_TOKEN="${1:?Usage: $0 <join-token>}"

echo "=== Step 1: Install k8s snap ==="
if snap list k8s &>/dev/null; then
    echo "k8s snap already installed."
else
    sudo snap install k8s --classic --channel=1.35-classic/stable
fi

echo "=== Step 2: Join cluster ==="
sudo k8s join-cluster "${JOIN_TOKEN}"

echo "=== Step 3: Setup kubectl alias ==="
if ! grep -q "alias k=" ~/.bash_aliases 2>/dev/null; then
    cat >> ~/.bash_aliases << 'EOF'
alias k='sudo k8s kubectl'
EOF
fi
source ~/.bash_aliases

echo "Done! Worker joined the cluster."
