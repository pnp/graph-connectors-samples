#!/usr/bin/env bash

# login
echo "Sign in to Microsoft 365..."
npx -p @pnp/cli-microsoft365 -- m365 login

# create AAD app
echo "Creating AAD app..."
appInfo=$(npx -p @pnp/cli-microsoft365 -- m365 aad app add --name "IBM -DB2 LOB - connector" --withSecret --apisApplication "https://graph.microsoft.com/ExternalConnection.ReadWrite.OwnedBy, https://graph.microsoft.com/ExternalItem.ReadWrite.OwnedBy" --grantAdminConsent --output json)

# write app to env.js
echo "Writing app to env.js..."
echo "export const appInfo = $appInfo;" > env.js

echo "DONE"
