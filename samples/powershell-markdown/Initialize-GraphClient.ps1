$config = Get-Content -Path "config.ini" | ConvertFrom-StringData
$credential = Get-Secret -Name "waldekblogpowershell"

Connect-MgGraph -ClientSecretCredential $credential -TenantId $config.TenantId -NoWelcome