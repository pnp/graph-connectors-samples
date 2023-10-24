# Ingest Microsoft 365 and Power Platform community samples using JavaScript and Node.js

## Summary

This samples contains a Microsoft Graph connector that shows how to bring LOB data to Microsoft 365, this case use IBM DB2 SALES data located on the IBM Cloud.   It gives you a very convenient way to search  Client Orders right from your tenant!

![IBMDB2](assets/preview01.png)

## Contributors

- [João Mendes](https://github.com/joaojmendes)

## Version history

Version|Date|Comments
-------|----|--------
1.0|October 23, 2023|Initial release

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [Node@18](https://nodejs.org)
- Bash

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/graph-connectors-samples/tree/main/samples/nodejs-javascript-solutiongallery) then unzip it)
- Make the setup script executable, by running `chmod +x ./setup.sh`
- Run the setup script: `./setup.sh`. When finished, it will create a local `env.js` file with information about the AAD app, required to run the code
- Restore dependencies: `npm install`
- Create the external connection: `npm run createConnection` (this will take several minutes)
- Ingest the content: `npm run loadContent`

## Features

This sample shows how to use query LOB Data from IBM DB2 located on IBM Cloud (external) into your Microsoft 365 tenant.

The sample illustrates the following concepts:

- script creating the Entra ID (Azure AD) application using CLI for Microsoft 365
- create external connection schema
- retrieve data from a remote API and add to the index
- logging errors for easy debugging
- visualize the external content in search results using a custom Adaptive Card
- extend Microsoft Graph JavaScript SDK with a [middleware to wait for a long-running operation to complete](https://blog.mastykarz.nl/easily-handle-long-running-operations-middleware-microsoft-graph-javascript-sdk/)
- extend Microsoft Graph JavaScript SDK with a [debug middleware](https://blog.mastykarz.nl/easily-debug-microsoft-graph-javascript-sdk-requests/) to show information about outgoing requests and incoming responses

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/graph-connectors-samples/issues?q=label%3A%22sample%3A%nodejs-javascript-solutiongallery%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/graph-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/graph-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-nodejs-javascript-solutiongallery)
