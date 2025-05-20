# Ingest local markdown content using Java

## Summary

This sample contains a Copilot connector built in Java that shows how to ingest local markdown files. For each file, it extracts the metadata from front matter, maps them to the external connection's schema and ingests the content retaining the content and metadata. The ingested content is set to be visible to everyone in the organization.

![Local markdown files displayed in Microsoft Search search results](assets/screenshot.png)

## Contributors

- [Waldek Mastykarz](https://github.com/waldekmastykarz)

## Version history

Version|Date|Comments
-------|----|--------
1.0|October 19, 2023|Initial release

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [Microsoft Graph PowerShell SDK](https://learn.microsoft.com/powershell/microsoftgraph/installation?view=graph-powershell-1.0)
- Microsoft PowerShell
- Java 17.0.8
- [jEnv](https://www.jenv.be/)
- [Gradle](https://gradle.org/) 8.4

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/copilot-connectors-samples/tree/main/samples/java-markdown) then unzip it)
- Follow the script:

    ```sh
    # create Entra app registration,
    # will store information in app/src/main/resources/connector/application.properties
    ./setup.sh
    # use Java 17.0.8 in the project
    jenv local 17.0.8
    # build project
    ./gradlew build
    # create connection
    ./gradlew run --args create-connection
    # load content
    ./gradlew run --args load-content
    ```

## Features

This sample shows how to ingest local markdown content with its front matter metadata to Microsoft 365. The sample contains a content folder with several blog posts by [Waldek Mastykarz](https://blog.mastykarz.nl/). These files are parsed and ingested by the sample Copilot connector.

The sample illustrates the following concepts:

- script creating the Entra (Azure AD) app registration using the Microsoft Graph PowerShell SDK
- create external connection including URL to item resolver to track activity when users share external links
- create external connection schema
- parse local markdown files to get their content and front matter metadata
- ingest content with initial activities
- visualize the external content in search results using a custom Adaptive Card
- extend Microsoft Graph Java SDK with a middleware to wait for a long-running operation to complete
- extend Microsoft Graph Java SDK with a debug middleware to show information about outgoing requests and incoming responses

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/copilot-connectors-samples/issues?q=label%3A%22sample%3A%java-markdown%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/copilot-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/copilot-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-java-markdown)
