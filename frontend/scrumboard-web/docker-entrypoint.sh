#!/bin/sh
set -eu

cat > /usr/share/nginx/html/assets/env.js <<EOF
window.__env = window.__env || {};
window.__env.apiUrl = '${API_URL:-/api}';
EOF

exec "$@"
