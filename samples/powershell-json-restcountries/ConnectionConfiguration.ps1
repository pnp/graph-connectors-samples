# Initialize to an empty hashtable to explicitly define the type as hashtable.
# This is needed to avoid the breaking change introduced in PowerShell 7.3 - https://github.com/PowerShell/PowerShell/issues/18524.
# https://github.com/microsoftgraph/msgraph-sdk-powershell/issues/2352
[hashtable]$adaptiveCard = @{}
$adaptiveCard += Get-Content -Path ".\resultLayout.json" -Raw | ConvertFrom-Json -AsHashtable

$externalConnection = @{
    userId     = "e1251b10-1ba4-49e3-b35a-933e3f21772b"
    connection = @{
        id               = "restcountries"
        name             = "REST Countries; PowerShell"
        description      = "Get information about countries"
        activitySettings = @{
            urlToItemResolvers = @(
                @{
                    "@odata.type" = "#microsoft.graph.externalConnectors.itemIdResolver"
                    urlMatchInfo  = @{
                        baseUrls   = @(
                            "https://restcountries.eu/rest/v2/name/"
                        )
                        urlPattern = "/(?<slug>[^/]+)"
                    }
                    itemId        = "{slug}"
                    priority      = 1
                }
            )
        }
        searchSettings   = @{
            searchResultTemplates = @(
                @{
                    id       = "restcountries"
                    priority = 1
                    layout   = @{
                        additionalProperties = $adaptiveCard
                    }
                }
            )
        }
    }
    
    # https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema     = @(
        @{
            name          = "name"
            type          = "String"
            isQueryable   = "true"
            isSearchable  = "true"
            isRetrievable = "true"
        }
        @{
            name          = "region"
            type          = "String"
            isQueryable   = "true"
            isSearchable  = "true"
            isRetrievable = "true"
        }
        @{
            name          = "subregion"
            type          = "String"
            isQueryable   = "true"
            isSearchable  = "true"
            isRetrievable = "true"
        }
        @{
            name          = "capital"
            type          = "String"
            isQueryable   = "true"
            isSearchable  = "true"
            isRetrievable = "true"
        }
        @{
            name          = "population"
            type          = "Int64"
            isRetrievable = "true"
        }
        @{
            name          = "latitude"
            type          = "Double"
            isRetrievable = "true"
        }
        @{
            name          = "longitude"
            type          = "Double"
            isRetrievable = "true"
        }
        @{
            name          = "areaInSqKm"
            type          = "Int64"
            isRetrievable = "true"
        }
        @{
            name          = "timezone"
            type          = "String"
            isRetrievable = "true"
        }
        @{
            name          = "mapUrl"
            type          = "String"
            isRetrievable = "true"
        }
        @{
            name          = "flagUrl"
            type          = "String"
            isRetrievable = "true"
        }
        @{
            name          = "borders"
            type          = "String"
            isRetrievable = "true"
        }
        @{
            name          = "languages"
            type          = "String"
            isRetrievable = "true"
        }
        @{
            name          = "currencies"
            type          = "String"
            isRetrievable = "true"
        }
    )
}
