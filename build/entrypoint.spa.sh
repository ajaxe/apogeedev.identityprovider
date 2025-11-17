#!/bin/sh

# Define defaults (optional, but good for safety)
DEFAULT_API_URL=""
DEFAULT_AUTHORITY="https://idsrv-1.apogee-dev.com"
DEFAULT_CLIENT_ID="local-dev-client"

# Use environment variables if present, otherwise use defaults
API_VAL=${API_URL_BASE:-$DEFAULT_API_URL}
AUTH_VAL=${OIDC_AUTHORITY:-$DEFAULT_AUTHORITY}
CLIENT_VAL=${OIDC_CLIENT_ID:-$DEFAULT_CLIENT_ID}

# Log the configuration for debugging (masking sensitive data if necessary)
echo "Generating config.js with:"
echo "  API_URL_BASE: $API_VAL"
echo "  OIDC_AUTHORITY: $AUTH_VAL"
echo "  OIDC_CLIENT_ID: $CLIENT_VAL"

# Write the config.js file
# Note: We use the path /usr/share/nginx/html because that is standard for the nginx image.
# If you changed the Nginx root, update this path.
cat <<EOF > /usr/share/nginx/html/config.js
window.APP_CONFIG = {
  API_URL_BASE: "${API_VAL}",
  OIDC_AUTHORITY: "${AUTH_VAL}",
  OIDC_CLIENT_ID: "${CLIENT_VAL}"
};
EOF

# Start the main process (Nginx)
exec "$@"
