# login
Write-Host "Sign in to Microsoft 365..."
npx -p @pnp/cli-microsoft365 -c 'm365 login'

# create AAD app
Write-Host "Creating AAD app..."
$appInfo=$(npx -p @pnp/cli-microsoft365 -- m365 entra app add --name "Sample Solution Gallery - connector" --withSecret --apisApplication "https://graph.microsoft.com/ExternalConnection.ReadWrite.OwnedBy, https://graph.microsoft.com/ExternalItem.ReadWrite.OwnedBy" --grantAdminConsent --output json)

# write app to env.js
Write-Host "Writing app to env.js..."
New-Item -ItemType File -Name "env.js" -Value "export const appInfo = $($appInfo | Out-String)" -Force

Write-Host "DONE"
