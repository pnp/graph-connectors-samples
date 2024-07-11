# login
Write-Host "Sign in to Microsoft 365..."
npx -p @pnp/cli-microsoft365 -- m365 login --authType browser

# create AAD app
Write-Host "Creating AAD app..."
$appInfo=$(npx -p @pnp/cli-microsoft365 -- m365 aad app add --name "MSGraph docs - connector" --withSecret --apisApplication "https://graph.microsoft.com/ExternalConnection.ReadWrite.OwnedBy, https://graph.microsoft.com/ExternalItem.ReadWrite.OwnedBy" --grantAdminConsent --output json)

# write app to env.ts
Write-Host "Writing app to src/env.ts..."
New-Item -ItemType File -Name "src/env.ts" -Value "export const appInfo = $($appInfo | Out-String)" -Force

Write-Host "DONE"
