# Policy copilot

## Summary

This sample project uses Teams Toolkit for Visual Studio Code to simplify the process of creating a [Microsoft Graph connector](https://learn.microsoft.com/graph/connecting-external-content-connectors-overview) that ingests data from a custom API to Microsoft Graph. It provides an end to end example of creating the connector, ingesting content and refreshing the ingested content. It also offers a declarative copilot to consume its content.

> [!WARNING]  
> This samples uses private preview capabilities of Copilot for Microsoft 365. You need to have access to the private preview to use this sample.

> [!NOTE]  
> Sample data was generated using Artificial Intelligence. Any resemblance to real data is purely coincidental.

![Data from custom API displayed in Copilot for Microsoft 365](./assets/connector-copilot-results.png)

## Contributors

- [Sébastien Levert](https://github.com/sebastienlevert)
- [Waldek Mastykarz](https://github.com/waldekmastykarz)

## Version History

Version|Date|Comments
-------|----|--------
1.0|August 19th, 2024|Initial release

## Prerequisites

- [Teams Toolkit for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=TeamsDevApp.ms-teams-vscode-extension)
- [Azure Functions Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program) with [uploading custom apps enabled](https://learn.microsoft.com/microsoftteams/platform/m365-apps/prerequisites#prepare-a-developer-tenant-for-testing)
- [Node@18](https://nodejs.org)

## Minimal path to awesome - Debug against a real Microsoft 365 tenant

### 1. Project setup

- Clone repo
- Open repo in VSCode
- Press <kbd>F5</kbd>, follow the sign in prompts
- When prompted, click on the link in the console to perform the tenant-wide admin consent
- Wait for all tasks to complete

### 2. Test in your declarative copilot

- Navigate to [Microsoft365.com/chat](https://www.microsoft365.com/chat)
- Select the declarative copilot named `Policy copilot`
- Use one of the suggested prompts
- A response from your declarative copilot will showcase the data ingested by the Graph connector.

![Empty connector copilot](./assets/connector-copilot.png)

### 3. Include data in results (Optional)

> This step is only required if you need to include the data in the search results in Microsoft 365 and in the general Copilot for Microsoft 365 experience. If you only want to use the connector data from a declarative copilot, you can skip this step.

- In the web browser navigate to the [Microsoft 365 admin center](https://admin.microsoft.com/)
- From the side navigation, open [Settings > Search & Intelligence](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch)
- On the page, navigate to the [Data Sources](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch/connectors) tab
- A table will display available connections. In the **Required actions** column, select the link to **Include Connector Results** and confirm the prompt
- Navigate to [Microsoft365.com/chat](https://www.microsoft365.com/chat)
- Use the following prompt: `What is the interest rate policy on Tatooine?`
- A response from Copilot for Microsoft 365 will showcase the data ingested by the Graph connector.

![Empty connector copilot](./assets/connector-copilot-results.png)

## Features

This sample shows how to ingest data from a custom API into your Microsoft 365 tenant.

The sample illustrates the following concepts:

- simplify debugging and provisioning of resources with Teams Toolkit for Visual Studio code
- create external connection schema
- support full ingestion of data
- visualize the external content in the Policy Management declarative copilot
- visualize the external content in Copilot for Microsoft 365
- visualize the external content in search results using a custom Adaptive Card

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/graph-connectors-samples/issues?q=label%3A%22sample%3A%nodejs-typescript-policies-dc%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/graph-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/graph-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-nodejs-typescript-policies-dc)
