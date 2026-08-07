#!/usr/bin/env bash
set -euo pipefail

echo "Requesting sudo access..."
sudo -v

echo "=== Step 0: Trust corp CA (if needed) ==="
TEST_HOST="canonical-bos01.cdn.snapcraftcontent.com"
if ! curl -sI "https://${TEST_HOST}" &> /dev/null; then
    echo | openssl s_client -showcerts -connect "${TEST_HOST}:443" 2>/dev/null | \
        sed -n '/-----BEGIN CERTIFICATE-----/,/-----END CERTIFICATE-----/p' | \
        sudo tee /usr/local/share/ca-certificates/corp-ca.crt > /dev/null
    sudo update-ca-certificates
    sudo systemctl restart snapd
fi

echo "=== Step 1: Install k8s snap ==="
if snap list k8s &>/dev/null; then
    echo "k8s snap already installed."
else
    sudo snap install k8s --classic --channel=1.35-classic/stable
fi

echo "=== Step 2: Bootstrap cluster ==="
if sudo k8s status &>/dev/null; then
    echo "Cluster already bootstrapped (status below)."
else
    sudo k8s bootstrap
fi

echo "=== Step 3: Wait for cluster ready ==="
sudo k8s status --wait-ready > /dev/null
echo "Cluster is ready."

echo "=== Step 4: Enable ingress ==="
if sudo k8s status | grep -q "ingress:.*enabled"; then
    echo "Ingress already enabled."
else
    sudo k8s enable ingress
fi

echo "=== Step 5: Setup kubectl alias ==="
if ! grep -q "alias k=" ~/.bash_aliases 2>/dev/null; then
    cat >> ~/.bash_aliases << 'EOF'
alias k='sudo k8s kubectl'
EOF
fi
echo "Alias 'k' configured."

echo "=== Step 6: Final cluster status ==="
sleep 5
sudo k8s status

exec bash
