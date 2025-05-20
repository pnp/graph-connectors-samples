<# 
----------------------------------------------------------------------------

Created:      Paul Bullock
Copyright (c) 2023
Date:         22/06/2023
Disclaimer:   

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

.Synopsis

.Example

.Notes

Useful reference: 
      List any useful references

 ----------------------------------------------------------------------------
#>

$cfgFileName = "config.ini"

#-----------------------------------------------------------
# Setup Graph Connections
#-----------------------------------------------------------

function SetupGraphConnectorEntraId{
    param(
        [string]$DisplayName,
        [string]$secretName
    )

    ##TODO Check Config file or display name exists for the connector
    Write-Host "Checking Copilot connector for Entra ID..." -ForegroundColor Blue

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

    $result = Get-MgApplication -Filter "DisplayName eq '$($DisplayName)'"

    if(!$result){
        Write-Host "Creating Copilot connector for Entra ID..." -NoNewline -ForegroundColor Blue
         # create the application
        $app = New-MgApplication -DisplayName $DisplayName -RequiredResourceAccess $requiredResourceAccess

        # grant admin consent
        $graphSpId = $(Get-MgServicePrincipal -Filter "appId eq '00000003-0000-0000-c000-000000000000'").Id
        $sp = New-MgServicePrincipal -AppId $app.appId
        New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "f431331c-49a6-499f-be1c-62af19c34a9d" -ResourceId $graphSpId
        New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $sp.Id -PrincipalId $sp.Id -AppRoleId "8116ae0f-55c2-452d-9944-d18420f5b2c8" -ResourceId $graphSpId

        # create client secret
        $cred = Add-MgApplicationPassword -ApplicationId $app.id
        
        $credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $app.appId, (ConvertTo-SecureString -String $cred.secretText -AsPlainText -Force)
        Set-Secret -Name $secretName -Secret $credential

        $config = "TenantId=$($(Get-MgContext).TenantId)"
        $config | Out-File -FilePath $cfgFileName -Encoding utf8

        Write-Host "...Done"
    }else{

        Write-Host "Retrieving credentials for existing app..." -NoNewLine
        $credential = Get-Secret -Name $secretName
        Write-Host "...Done"
    }

    return $credential
}

function InitializeGraphConnection{
    param(
        [string]$secretName
    )

    Write-Host "Initialise the Graph connection..." -ForegroundColor Blue

    $config = Get-Content -Path $cfgFileName | ConvertFrom-StringData
    $credential = Get-Secret -Name $secretName

    Connect-MgGraph -ClientSecretCredential $credential -TenantId $config.TenantId -NoWelcome
}

#-----------------------------------------------------------
# Connection Configuration
#-----------------------------------------------------------

function Get-AdaptiveCard{
    
    Write-Host "Getting Adaptive Card from $(Get-Location)\resultLayout.json"

    # Initialize to an empty hashtable to explicitly define the type as hashtable.
    # This is needed to avoid the breaking change introduced in PowerShell 7.3 - https://github.com/PowerShell/PowerShell/issues/18524.
    # https://github.com/microsoftgraph/msgraph-sdk-powershell/issues/2352
    [hashtable]$adaptiveCard = @{}
    $adaptiveCard += Get-Content -Path "$(Get-Location)\resultLayout.json" -Raw | ConvertFrom-Json -AsHashtable

    return $adaptiveCard
}

function CreateExternalConnection{
    param(
        [PSCustomObject] $externalConnection
    )
    
    Write-Host "Creating external connection..." -NoNewLine -ForegroundColor Blue
    New-MgExternalConnection -BodyParameter $externalConnection.connection -ErrorAction Stop
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
}

#-----------------------------------------------------------
# Process Content for Import
#-----------------------------------------------------------


function Import-ExternalItems {
    param(
        [Object[]] $ExternalItems,
        [PSCustomObject] $ExternalConnection
    )

    Write-Host "Starting to import items..."

    $count = $ExternalItems.Count
    $i = 0

    $ExternalItems | ForEach-Object {
        try{
            Set-MgExternalConnectionItem -ExternalConnectionId $externalConnection.connection.id -ExternalItemId $_.id -BodyParameter $_ -ErrorAction Stop | Out-Null
            $complete = [math]::Round((++$i/$count)*100, 0)
            Write-Progress -Activity "Importing items" -Status "$complete% Complete: $($_.id)" -PercentComplete $complete
        }catch{
            Write-Host "Error importing item: $($_.id) $_" -ForegroundColor Red
        }
        
    }
}

function GetUserId{
    param{
        [string]$UserPrincipalName
    }

    # TODO: Finish this method
    # Get the user ID from Microsoft Graph
    $user = Get-MgUser -Filter "userPrincipalName eq '$($UserPrincipalName)'"
    $user 

    return $user.Id
}