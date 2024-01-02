$ErrorActionPreference = "Stop"

Connect-MgGraph -Scopes AppRoleAssignment.ReadWrite.All,Application.ReadWrite.All -NoWelcome

$requiredResourceAccess = (@{
  "resourceAccess" = (
    @{
      id = "f431331c-49a6-499f-be1c-62af19c34a9d"
      type = "Role"
    },
    @{
      id = "8116ae0f-55c2-452d-9944-d18420f5b2c8"
      type = "Role"
    }
  )
  "resourceAppId" = "00000003-0000-0000-c000-000000000000"
})

# create the application
$app = New-MgApplication -DisplayName "O365C-GraphConnector-MicrosoftLearnAPI" -RequiredResourceAccess $requiredResourceAccess

# grant admin consent
$graphSpId = $(Get-MgServicePrincipal -Filter "appId eq '00000003-0000-0000-c000-000000000000'").Id
$sp = New-MgServicePrincipal -AppId $app.appId
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "f431331c-49a6-499f-be1c-62af19c34a9d" -ResourceId $graphSpId
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "8116ae0f-55c2-452d-9944-d18420f5b2c8" -ResourceId $graphSpId

# create client secret
$cred = Add-MgApplicationPassword -ApplicationId $app.id



# store values in user secrets
#dotnet user-secrets init
#dotnet user-secrets set "settings:ClientId" $app.appId
Write-Host "ClientId: $($app.appId)"
#dotnet user-secrets set "settings:ClientSecret" $cred.secretText
Write-Host "ClientSecret: $($cred.secretText)"
#dotnet user-secrets set "settings:TenantId" $($(Get-MgContext).TenantId)
Write-Host "TenantId: $($(Get-MgContext).TenantId)"
