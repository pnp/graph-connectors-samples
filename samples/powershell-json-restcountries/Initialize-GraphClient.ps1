$config = Get-Content -Path "config.ini" | ConvertFrom-StringData
$credential = Get-Secret -Name "restcountriespowershell"

Connect-MgGraph -ClientSecretCredential $credential -TenantId $config.TenantId 