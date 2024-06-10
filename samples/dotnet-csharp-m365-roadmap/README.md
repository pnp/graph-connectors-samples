# Ingest M365 Roadmap content in Microsoft 365 using Microsoft Graph connectors using C# and .NET

## Summary

This sample contains a Microsoft Graph connector that shows how to Ingest M365 Roadmap content in Microsoft 365 using Microsoft Graph connectors. For each file, it M365 Roadmap content, maps them to the external connection's schema and ingests the content retaining the content and metadata. The ingested content is set to be visible to everyone in the organization.

![M365 Roadmap Graph Connector](./assets/M365-Roadmap-Graph-Connector01.png)
![M365 Roadmap Graph Connector](./assets/M365-Roadmap-Graph-Connector02.png)


## Contributors

- [Mohammad Amer](https://github.com/mohammadamer)

## Version history

Version|Date|Comments
-------|----|--------
1.0|February 18, 2024|Initial release
1.1|May 12, 2024|Updated README.md, resultsLayout files
1.2|June 09, 2024|Updated README.md, upgraded Azure.Identity package

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft PowerShell
- Created and Configure Microsoft Entra app registration. Grant it the following API permissions
  - ExternalConnection.ReadWrite.OwnedBy
  - ExternalItem.ReadWrite.OwnedBy

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/graph-connectors-samples/tree/main/samples/dotnet-csharp-m365-roadmap) then unzip it)
- add the information about the Entra ID app in user secrets
  - dotnet user-secrets init
  - dotnet user-secrets set "EntraId:ClientId" "[application id]"
  - dotnet user-secrets set "EntraId:ClientSecret" "[secret value]"
  - dotnet user-secrets set "EntraId:TenantId" "[directory (tenant) id]"
- Build the project: `dotnet build`
- Change the working directory to the path to where the project file is present.
- Create the external connection: `dotnet run -- provision-connection` (this will take several minutes)
- Ingest the content: `dotnet run -- load-content`
- [Create result type](https://learn.microsoft.com/microsoftsearch/manage-result-types) with default settings and the external connection you've just created
- Use the `resultLayout.json` file for the Adaptive Card code

## Results Layout
- Browse to the admin center then Search & intelligence then update the result layout with the resultLayout json file.

## Features

This sample shows how to Ingest M365 Roadmap content in Microsoft 365 using Microsoft Graph connectors using C# and .NET

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-dotnet-csharp-m365-roadmap)
