#!/usr/bin/env bash
#
# Install and configure Canonical's k8s snap on this machine, either as a control-plane (cp) node or
# as a worker joining an existing cluster.
#
# Usage:
#   install-k8s-snap.sh [cp|worker] [worker-name|join-token]
#
#   install-k8s-snap.sh                         Install as control-plane (default)
#   install-k8s-snap.sh cp                      Same as above, explicit
#   install-k8s-snap.sh cp my-node              Also print a join token for a worker named "my-node"
#   install-k8s-snap.sh worker <join-token>     Join an existing cluster as a worker
#
set -euo pipefail

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
    tail -n +2 "$0" | grep '^#' | sed 's/^#//; s/^ //'
    exit 0
fi

ROLE="${1:-cp}"
EXTRA="${2:-}"

if [[ "$ROLE" != "cp" && "$ROLE" != "worker" ]]; then
    echo "Usage: $0 [cp|worker] [join-token]" >&2
    echo "  role defaults to 'cp' if omitted." >&2
    exit 1
fi
 
if [[ "$ROLE" == "worker" && -z "$EXTRA" ]]; then
    echo "Error: worker role requires a join token." >&2
    echo "Usage: $0 worker <join-token>" >&2
    exit 1
fi

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

if [[ "$ROLE" == "worker" ]]; then

    JOIN_TOKEN="$EXTRA"

    echo "=== Step 2: Join cluster as worker ==="
    if sudo k8s status &>/dev/null; then
        echo "Node already part of a cluster."
    else
        sudo k8s join-cluster "$JOIN_TOKEN"
    fi
 
    echo "=== Step 3: Final cluster status ==="
    sudo k8s status

else

    WORKER_NAME="$EXTRA"

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
    mkdir -p ~/.kube
    sudo k8s kubectl completion bash > ~/.kube/k8s-completion.bash
    if ! grep -q "alias k=" ~/.bash_aliases 2>/dev/null; then
        cat >> ~/.bash_aliases << 'EOF'
alias k='sudo k8s kubectl'
[ -f ~/.kube/k8s-completion.bash ] && source ~/.kube/k8s-completion.bash
complete -o default -F __start_kubectl k
EOF
    fi
    echo "Alias 'k' and autocompletion configured."

    sleep 5

    echo "=== Step 6: Final cluster status ==="
    sudo k8s status

    if [[ -n "$WORKER_NAME" ]]; then
        echo "=== Step 7: Generate join token for worker '$WORKER_NAME' ==="
        sudo k8s get-join-token "$WORKER_NAME" --worker
    fi

fi

exec bash
