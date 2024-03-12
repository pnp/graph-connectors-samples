# Ingest local markdown content using JavaScript and Node.js

## Summary

This sample contains a Microsoft Graph connector that shows how to ingest local markdown files. For each file, it extracts the metadata from front matter, maps them to the external connection's schema and ingests the content retaining the content and metadata. The ingested content is set to be visible to everyone in the organization.

![Local markdown files displayed in Microsoft Search search results](assets/screenshot.png)

## Contributors

- [Waldek Mastykarz](https://github.com/waldekmastykarz)

## Version history

Version|Date|Comments
-------|----|--------
2.1|March 12, 2024|Fixed schema
2.0|March 11, 2024|Added simulating handling Teams Admin Center notification
1.1|October 09, 2023|Added configuring result layout
1.0|September 29, 2023|Initial release

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [Node@18](https://nodejs.org)
- Bash

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/graph-connectors-samples/tree/main/samples/nodejs-javascript-markdown) then unzip it)
- Make the setup script executable, by running `chmod +x ./setup.sh`
- Run the setup script: `./setup.sh`. When finished, it will create a local `env.js` file with information about the AAD app, required to run the code
- Restore dependencies: `npm install`
- Create the external connection: `npm run createConnection` (this will take several minutes)
- Ingest the content: `npm run loadContent`

## Features

This sample shows how to ingest local markdown content with its front matter metadata to Microsoft 365. The sample contains a content folder with several blog posts by [Waldek Mastykarz](https://blog.mastykarz.nl/). These files are parsed and ingested by the sample Microsoft Graph connector.

The sample illustrates the following concepts:

- script creating the Entra (Azure AD) app registration using CLI for Microsoft 365
- create external connection including URL to item resolver to track activity when users share external links
- create external connection schema
- parse local markdown files to get their content and front matter metadata
- ingesting content with initial activities
- visualize the external content in search results using a custom Adaptive Card
- extend Microsoft Graph JavaScript SDK with a middleware to wait for a long-running operation to complete
- extend Microsoft Graph JavaScript SDK with a [debug middleware](https://blog.mastykarz.nl/easily-debug-microsoft-graph-javascript-sdk-requests/) to show information about outgoing requests and incoming responses
- use Dev Proxy mocks to simulate creation of the external connection and its schema

If you want to use Dev Proxy to simulate API responses:

- install Dev Proxy v0.16.0 or later
- download the [`microsoft-graph-connector-notification`](https://github.com/pnp/proxy-samples/tree/main/samples/microsoft-graph-connector-notification) preset using `devproxy preset get microsof-tgraph-connector-notification`
- check out the sample readme for additional information and usage instructions
  - `NODE_TLS_REJECT_UNAUTHORIZED=0` is required to skip certificate validation errors when using the Dev Proxy
  - `NODE_NO_WARNINGS=1` suppresses Node.js warnings about skipping certificate validation
  - These environment variables are included in the `:proxy`-suffixed scripts in `package.json`.
- instead of running the `setup.sh` script, rename the `simulated.env.js` file to `env.js` to use the simulated environment
- start Dev Proxy using the `npm start:proxy` script
- you can simulate creating the connection, provisioning schema and ingesting content in two ways:
  - on-demand, using the `createConnection:proxy` and `loadContent:proxy` scripts
  - simulating the Teams Admin Center notification, using the `start:notification` script to start the API that handles the TAC notification, and simulating the notification from Dev Proxy, by pressing `w`

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/graph-connectors-samples/issues?q=label%3A%22sample%3A%nodejs-javascript-markdown%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/graph-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/graph-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-nodejs-javascript-markdown)
