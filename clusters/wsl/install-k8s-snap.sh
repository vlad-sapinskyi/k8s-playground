#!/usr/bin/env bash
set -euo pipefail

echo "=== Step 0: trust corp CA (if needed) ==="
TEST_HOST="canonical-bos01.cdn.snapcraftcontent.com"
if ! curl -sI "https://${TEST_HOST}" &> /dev/null; then
    echo | openssl s_client -showcerts -connect "${TEST_HOST}:443" 2>/dev/null | \
        sed -n '/-----BEGIN CERTIFICATE-----/,/-----END CERTIFICATE-----/p' | \
        sudo tee /usr/local/share/ca-certificates/corp-ca.crt > /dev/null
    sudo update-ca-certificates
    sudo systemctl restart snapd
fi

echo "=== Step 1: k8s snap ==="
if snap list k8s &> /dev/null; then
    echo "k8s snap already installed."
else
    echo "Installing k8s snap..."
    sudo snap install k8s --classic
fi

echo "=== Step 2: bootstrap cluster ==="
if sudo k8s status &> /dev/null; then
    echo "Cluster already bootstrapped (status below)."
else
    echo "Bootstrapping cluster..."
    sudo k8s bootstrap
fi

echo "=== Step 3: enable ingress addons ==="
echo "Enabling ingress addon..."
sudo k8s enable ingress

sleep 5
sudo k8s status
