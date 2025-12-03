#!/bin/sh

# Start the Node.js environment server in the background
node /app/env-server.js &

# Start nginx in the foreground
nginx -g "daemon off;"