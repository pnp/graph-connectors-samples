<# 
----------------------------------------------------------------------------

Created:      Paul Bullock
Copyright (c) 2023
Date:         19/11/2023
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

[CmdletBinding()]
param (
    [switch]$Initialize
)
begin{

    # Import the functions
    . .\Functions.ps1

    $baseExternalUrl = "https://ignite.microsoft.com/en-US/sessions/"
    $jsonContent = Get-Content "content\ignite-sessions.json" -Raw | ConvertFrom-Json
    $entraIdDisplayName = "Paul Bullock (Ignite 2023) - connector (PowerShell)"
    $secretName = "pkbignite2023powershell"
}
process {

    # Load the adaptive card file
    $adaptiveCard = Get-AdaptiveCard

    # Define the external connection and the schema for search items structure
    $externalConnection = @{
        userId     = "d47e12f9-99f3-40ea-8870-7b39d2be92f7" # From Azure Entra ID
        # The name of the connection, as it will appear in the Microsoft Search admin center
        # Defines the details of the connection
        connection = @{
            id               = "pkbignite2023sessions"
            name             = "PKB Ignite 2023 Sessions"
            description      = "Content and sessions for Ignite 2023, listing the session information from Ignite"
            activitySettings = @{
                urlToItemResolvers = @(
                    @{
                        "@odata.type" = "#microsoft.graph.externalConnectors.itemIdResolver"
                        urlMatchInfo  = @{
                            baseUrls   = @(
                                "$($baseExternalUrl)"
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
                        id       = "pkbignitepwsh"
                        priority = 1
                        layout   =  $adaptiveCard
                    }
                )
            }
        }
        # The schema is a way of defining the columns that will be available in the search results and how they are mapped to the content
        # https://learn.microsoft.com/graph/connecting-external-content-manage-schema
        schema     = @(
            @{
                name          = "title"
                type          = "String"
                isQueryable   = "true"
                isSearchable  = "true"
                isRetrievable = "true"
                labels        = @(
                    "title"
                )
            }
            @{
                name          = "excerpt"
                type          = "String"
                isQueryable   = "true"
                isSearchable  = "true"
                isRetrievable = "true"
            }            
            @{
                name          = "url"
                type          = "String"
                isRetrievable = "true"
                labels        = @(
                    "url"
                )
            }
            @{
                name          = "sessionCode"
                type          = "String"
                isQueryable   = "true"
                isRetrievable = "true"
                isSearchable  = "true"
            }
            @{
                name          = "date"
                type          = "DateTime"
                isQueryable   = "true"
                isRetrievable = "true"
                isSearchable  = "false"
                labels        = @(
                    "lastModifiedDateTime"
                )
            }
            @{
                name          = "tags"
                type          = "StringCollection"
                isQueryable   = "true"
                isRetrievable = "true"
                isRefinable   = "true"
            }
        )
    }

    $externalItemsToAdd = @()

    $jsonContent | ForEach-Object {

        $startDate = $_.startDateTime
        if ($startDate -eq $null) {
            $startDate = "2023-11-15T00:00:00+00:00"
        }

        $sessionDate = Get-Date $startDate -Format "yyyy-MM-ddTHH:mm:ssZ"

        $externalItemsToAdd += @{
            id = $_.sessionId
            properties = @{
                title = $_.title
                excerpt = $_.description
                url = [System.Uri]::new([System.Uri]$baseExternalUrl, $_.sessionId).ToString()
                date = $sessionDate
                sessionCode = $_.sessionCode
                "tags@odata.type" = "Collection(String)"
                tags = $_.contentArea
            }
            content = @{
                # TODO Future Enhancement, transcribe the video content to form part of the content, store as Markdown
                value = $_.description
                type = 'text'
            }
            acl = @(
                @{
                    accessType = "grant"
                    type = "everyone"
                    value = "everyone"
                }
            )
            # If you'd like to add the created activity,
            # in the config.js file, add the userId property mapped to your user's ID
            # and then uncomment the following lines:
            activities = @(@{
                "@odata.type" = "#microsoft.graph.externalConnectors.externalActivity"
                type = "created"
                startDateTime = $sessionDate
                performedBy = @{
                    type = "user"
                    id = $externalConnection.userId
                }
            })
        }
    }
   
    # Finally, run through the process of creating the connection, schema and importing the content
    # Create Entra app
    SetupGraphConnectorEntraId -secretName $secretName -displayName $entraIdDisplayName
    
    # Initialize Graph connection
    InitializeGraphConnection -secretName $secretName
    
    # Create external connection
    CreateExternalConnection -externalConnection $externalConnection

    # Import content
    Import-ExternalItems -ExternalConnection $externalConnection -ExternalItems $externalItemsToAdd
}
end{

  Write-Host "Done! :)" -ForegroundColor Green
}