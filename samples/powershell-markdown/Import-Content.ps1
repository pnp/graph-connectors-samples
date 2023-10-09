. ".\Initialize-GraphClient.ps1"
. ".\ConnectionConfiguration.ps1"

function Read-Content {
    $files = Get-ChildItem -Path ".\content" -Recurse -File -Filter "*.markdown"
    $files | ForEach-Object {
        $content = Get-Content -Path $_.FullName -Raw
        $post = @{
            Content = $content
        }

        # extract frontmatter
        $frontmatterRegex = "(?s)^---\r?\n(.*?)\r?\n---\r?\n"
        $match = [regex]::Match($content, $frontmatterRegex)

        if ($match.Success) {
            $frontmatter = $match.Groups[1].Value
            $post.Meta = ConvertFrom-Yaml $frontmatter

            $post.Content = $($content -replace $frontmatterRegex, "" | ConvertFrom-Markdown).Html
        }

        Write-Output $post
    }
}

function ConvertTo-ExternalItem {
    param(
        [Object[]] $Content
    )

    $baseUrl = "https://blog.mastykarz.nl";

    $Content | ForEach-Object {
        $docDate = Get-Date $_.Meta.date -Format "yyyy-MM-ddTHH:mm:ssZ"
        $item = @{
            id = $_.Meta.slug
            properties = @{
              title = $_.Meta.title
              excerpt = $_.Meta.excerpt
              imageUrl = [System.Uri]::new([System.Uri]$baseUrl, $_.Meta.image).ToString()
              url = [System.Uri]::new([System.Uri]$baseUrl, $_.Meta.slug).ToString()
              date = $docDate
              "tags@odata.type" = "Collection(String)"
              tags = $_.Meta.tags
            }
            content = @{
              value = $_.Content
              type = 'html'
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
              startDateTime = $docDate
              performedBy = @{
                type = "user"
                id = $externalConnection.userId
              }
            })
        }

        Write-Output $item
    }
}

function Import-ExternalItems {
    param(
        [Object[]] $ExternalItems
    )

    $count = $ExternalItems.Count
    $i = 0

    $ExternalItems | ForEach-Object {
        Set-MgExternalConnectionItem -ExternalConnectionId $externalConnection.connection.id -ExternalItemId $_.id -BodyParameter $_ -ErrorAction Stop | Out-Null
        $complete = [math]::Round((++$i/$count)*100, 0)
        Write-Progress -Activity "Importing items" -Status "$complete% Complete: $($_.id)" -PercentComplete $complete
    }
}

$content = Read-Content
$externalItems = ConvertTo-ExternalItem -Content $content
Import-ExternalItems -ExternalItems $externalItems