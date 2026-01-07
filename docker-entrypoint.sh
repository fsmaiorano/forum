#!/bin/bash
set -e

# Generate self-signed certificate if it doesn't exist
if [ ! -f /https/aspnetapp.pfx ]; then
    echo "Generating self-signed HTTPS certificate..."
    dotnet dev-certs https -ep /https/aspnetapp.pfx -p password --trust 2>/dev/null || \
    dotnet dev-certs https -ep /https/aspnetapp.pfx -p password
    echo "Certificate generated successfully!"
fi

# Start the application
exec dotnet "$@"

