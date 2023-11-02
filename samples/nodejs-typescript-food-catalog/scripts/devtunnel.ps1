$exe = where.exe devtunnel.exe
if ("" -eq $exe) {
    Write-Host "Dev Tunnels CLI not found. Please install: https://learn.microsoft.com/azure/developer/dev-tunnels/get-started"
    exit
}

$tunnnelId = ""
$envFile = "env/.env.local"
$envFileContent = Get-Content $envFile
$envFileContent | ForEach-Object {
    $envFileContent | ForEach-Object {
        if ($_ -like "TUNNEL_ID=*") {
            $tunnelId = $_.Split("=")[1].Trim()
        }
    }
}

if ("" -eq $tunnelId) {
    Write-Host "No TUNNEL_ID found. Creating tunnel..."

    Write-Host "Logging in to Dev Tunnels..."
    $login = devtunnel user login
    
    Write-Host "Creating tunnel..."
    $tunnel = devtunnel.exe create
    $tunnelId = $tunnel -split '\r?\n' | Select-String 'Tunnel ID' | ForEach-Object { ($_ -split ':')[1].Trim() }
    
    Write-Host "Creating port and access..."
    $port = 7071
    $createPort = devtunnel port create $tunnelId -p $port
    $createAccess = devtunnel access create $tunnelId -p $port -a
    
    Write-Host "Updating env\.env.local..."

    $hostname = $tunnelId.split('.')[0]
    $cluster = $tunnelId.split('.')[1]

    $domain = "$hostname-$port.$cluster.devtunnels.ms"
    $endpoint = "https://$domain"

    $envFileContent | ForEach-Object {
        if ($_ -like "NOTIFICATION_ENDPOINT=*") {
            $_ = "NOTIFICATION_ENDPOINT=$endpoint"
        }
        if ($_ -like "NOTIFICATION_DOMAIN=*") {
            $_ = "NOTIFICATION_DOMAIN=$domain"
        }
        if ($_ -like "TUNNEL_ID=*") {
            $_ = "TUNNEL_ID=$tunnelId"
        }
        $_
    } | Set-Content $envFile

    Write-Host "TUNNEL_ID: $tunnelId"
    Write-Host "NOTIFICATION_ENDPOINT: $endpoint"
    Write-Host "NOTIFICATION_DOMAIN: $domain"
}

devtunnel.exe host $tunnelId
