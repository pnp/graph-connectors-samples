# Ingest local markdown content using CLI for Microsoft 365, zx and Node.js

## Summary

This sample contains a sample zx script using CLI for Microsoft 365 that shows how to create an external connection with a schema and ingest external content to it. For each file, it extracts the metadata from front matter, maps them to the external connection's schema and ingests the content retaining the content and metadata. The ingested content is set to be visible to everyone in the organization.

![Local markdown files displayed in Microsoft Search search results](assets/screenshot.png)

## Contributors

- [Waldek Mastykarz](https://github.com/waldekmastykarz)

## Version history

Version|Date|Comments
-------|----|--------
1.0|September 21, 2024|Initial release

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [Node@20](https://nodejs.org)
- [zx](https://google.github.io/zx/)
- [CLI for Microsoft 365](https://aka.ms/cli-m365) signed in with `ExternalConnection.ReadWrite.*` and `ExternalItem.ReadWrite.*` permissions

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/graph-connectors-samples/tree/main/samples/nodejs-zx_clim365-markdown) then unzip it)
- Make the script executable, by running `chmod +x ./index.js`
- Run the script: `./index.js`. The script creates an external connection, its schema, and ingests the content from the `content` folder

## Features

This sample shows how to ingest local markdown content with its front matter metadata to Microsoft 365. The sample contains a content folder with several blog posts by [Waldek Mastykarz](https://blog.mastykarz.nl/). These files are parsed and ingested by the sample Microsoft Graph connector implemented using CLI for Microsoft 365.

The sample illustrates the following concepts:

- create external connection including URL to item resolver to track activity when users share external links
- create external connection schema
- parse local markdown files to get their content and front matter metadata
- ingesting content with initial activities
- use CLI for Microsoft 365 to create the external connection, its schema, and ingest the content
- use zx to create a script that uses CLI for Microsoft 365

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/graph-connectors-samples/issues?q=label%3A%22sample%3A%nodejs-zx_clim365-markdown%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/graph-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/graph-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-nodejs-zx_clim365-markdown)
