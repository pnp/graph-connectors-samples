. ".\Initialize-GraphClient.ps1"
. ".\ConnectionConfiguration.ps1"


function Get-RESTCountries {
  param(
    [string] $Url
  )

  $restCountries = @()

  $json = Invoke-RestMethod -Uri "$Url/all" -Method Get -ContentType "application/json"
  $index = 1

  $json | ForEach-Object {
    $country = $_

    foreach ($border in $country.borders) {
      $borderCountry = Invoke-RestMethod -Uri "$Url/alpha/$border" -Method Get -ContentType "application/json"
      if ($borderCountry.Count -gt 0) {
        $borderCountries += $borderCountry[0].common
      }
    }

    Write-Host "Retrieving $($country.name.common)... $index of $($json.Count)" -ForegroundColor Yellow
    # Create custom object and add to $restCountries array
    $restCountries += [pscustomobject]@{
      Name       = $country.name.common
      Region     = $country.region
      Subregion  = $country.subregion
      Capital    = if ($country.capital) { [string]::join(", ", $country.capital) } else { '' }
      Population = $country.population
      Latitude   = $country.latlng[0]
      Longitude  = $country.latlng[1]
      AreaInSqKm = $country.area
      Timezone   = if ($country.timezones) { [string]::join(", ", $country.timezones) } else { '' }
      Map        = $country.maps.googleMaps
      Flag       = $country.flags.png
      Borders    = if ($borderCountries) { [string]::join(", ", $borderCountries) } else { '' }
      Languages  = if ($country.languages.psobject.properties.value) { [string]::join(", ", $country.languages.psobject.properties.value) } else { '' }
      Currencies = if ($country.currencies.psobject.properties.value.name) { [string]::join(", ", $country.currencies.psobject.properties.value.name) } else { '' }
    }

    $index++
  }

  return $restCountries
}

function Import-ExternalItems {
  param(
    [Object[]] $Content
  )

  $startDate = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"

  $index = 1
  $Content | ForEach-Object {
    $item = @{
      id         = $index
      properties = @{
        name       = $_.Name
        region     = $_.Region
        subregion  = $_.Subregion
        capital    = $_.Capital
        population = $_.Population
        latitude   = $_.Latitude
        longitude  = $_.Longitude
        areaInSqKm = $_.AreaInSqKm
        timezone   = $_.Timezone
        mapUrl     = [System.Uri]::new($_.Map).ToString()
        flagUrl    = [System.Uri]::new($_.Flag).ToString()
        borders    = $_.Borders
        languages  = $_.Languages
        currencies = $_.Currencies
      }
      content    = @{
        value = $_.Name
        type  = 'text'
      }
      acl        = @(
        @{
          accessType = "grant"
          type       = "everyone"
          value      = "everyone"
        }
      )
          
      activities = @(@{
          "@odata.type" = "#microsoft.graph.externalConnectors.externalActivity"
          type          = "created"
          startDateTime = $startDate
          performedBy   = @{
            type = "user"
            id   = $externalConnection.userId
          }
        })
    }

    try {
      Set-MgExternalConnectionItem -ExternalConnectionId $externalConnection.connection.id -ExternalItemId $item.id -BodyParameter $item -ErrorAction Stop | Out-Null
      Write-Host "Imported $($item.properties.name)... $index of $($Content.Count)" -ForegroundColor Green  
    }
    catch {
      Write-Error "Failed to import $($item.properties.name)"
      Write-Error $_.Exception.Message
    }
    
    $index++
  }
}

$content = Get-RESTCountries -Url "https://restcountries.com/v3.1"
Import-ExternalItems -Content $content