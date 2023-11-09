Connect-MgGraph -Scopes AppRoleAssignment.ReadWrite.All,Application.ReadWrite.All -NoWelcome -ErrorAction Stop

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
$app = New-MgApplication -DisplayName "MSGraph docs - connector" -RequiredResourceAccess $requiredResourceAccess -ErrorAction Stop

# grant admin consent
$graphSpId = $(Get-MgServicePrincipal -Filter "appId eq '00000003-0000-0000-c000-000000000000'" -ErrorAction Stop).Id
$sp = New-MgServicePrincipal -AppId $app.appId -ErrorAction Stop
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "f431331c-49a6-499f-be1c-62af19c34a9d" -ResourceId $graphSpId -ErrorAction Stop
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "8116ae0f-55c2-452d-9944-d18420f5b2c8" -ResourceId $graphSpId -ErrorAction Stop

# create client secret
$cred = Add-MgApplicationPassword -ApplicationId $app.id -ErrorAction Stop

# store values in user secrets
dotnet user-secrets init
dotnet user-secrets set "AzureAd:ClientId" $app.appId
dotnet user-secrets set "AzureAd:ClientSecret" $cred.secretText
dotnet user-secrets set "AzureAd:TenantId" $($(Get-MgContext).TenantId)