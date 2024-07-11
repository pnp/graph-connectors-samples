#!/usr/bin/env bash

# login
echo "Sign in to Microsoft 365..."
npx -p @pnp/cli-microsoft365 -- m365 login --authType browser

# create Entra app
echo "Creating Entra app..."
appInfo=$(npx -p @pnp/cli-microsoft365 -- m365 entra app add --name "MSGraph docs - connector" --withSecret --apisApplication "https://graph.microsoft.com/ExternalConnection.ReadWrite.OwnedBy, https://graph.microsoft.com/ExternalItem.ReadWrite.OwnedBy" --grantAdminConsent --output json)

# write app to env.ts
echo "Writing app to src/env.ts..."
echo "export const appInfo = $appInfo;" > src/env.ts

echo "DONE"