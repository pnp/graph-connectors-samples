. ".\Initialize-GraphClient.ps1"
. ".\ConnectionConfiguration.ps1"

Write-Host "Creating external connection..." -NoNewLine
New-MgExternalConnection -BodyParameter $externalConnection.connection -ErrorAction Stop | Out-Null
Write-Host "DONE" -ForegroundColor Green

Write-Host "Creating schema..." -NoNewLine
$body = @{
    baseType = "microsoft.graph.externalItem"
    properties = $externalConnection.schema
}
Update-MgExternalConnectionSchema -ExternalConnectionId $externalConnection.connection.id -BodyParameter $body -ErrorAction Stop
Write-Host "DONE" -ForegroundColor Green

Write-Host "Waiting for the schema to get provisioned..." -ForegroundColor Yellow -NoNewline
do {
    $connection = Get-MgExternalConnection -ExternalConnectionId $externalConnection.connection.id
    Start-Sleep -Seconds 60
    Write-Host "." -NoNewLine -ForegroundColor Yellow
} while ($connection.State -eq 'draft')

Write-Host "DONE" -ForegroundColor Green
