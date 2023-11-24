# Ingest local markdown content using Python

## Summary

This samples contains a Microsoft Graph connector built in Python that shows how to ingest local markdown files. For each file, it extracts the metadata from front matter, maps them to the external connection's schema and ingests the content retaining the content and metadata. The ingested content is set to be visible to everyone in the organization.

![Local markdown files displayed in Microsoft Search search results](assets/screenshot.png)

## Contributors

- [Waldek Mastykarz](https://github.com/waldekmastykarz)

## Version history

Version|Date|Comments
-------|----|--------
1.2|October 11, 2023|Updated logging and formatting
1.1|October 9, 2023|Added configuring result layout
1.0|October 9, 2023|Initial release

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [Microsoft Graph CLI](https://devblogs.microsoft.com/microsoft365dev/microsoft-graph-cli-v1-0-0-release-candidate-now-with-beta-support/)
- [jq](https://jqlang.github.io/jq/)
- [pyenv](https://github.com/pyenv/pyenv)

## Minimal path to awesome

- Clone this repository (or [download this solution as a .ZIP file](https://pnp.github.io/download-partial/?url=https://github.com/pnp/graph-connectors-samples/tree/main/samples/python-markdown) then unzip it)
- Follow the script:

    ```sh
    # make the setup script executable
    chmod +x ./setup.sh
    # create Entra app
    ./setup.sh
    # ensure you've got Python 3.11 installed
    pyenv install 3.11
    # use Python 3.11 in the project
    pyenv local 3.11
    # create virtual environment
    python3 -m venv venv
    # activate virtual environment
    source venv/bin/activate
    # restore dependencies
    pip install -r requirements.txt
    # create connection
    python3 main.py create-connection
    # load content
    python3 main.py load-content
    # deactivate virtual environment
    deactivate
    ```

## Features

This sample shows how to ingest local markdown content with its front matter metadata to Microsoft 365. The sample contains a content folder with several blog posts by [Waldek Mastykarz](https://blog.mastykarz.nl/). These files are parsed and ingested by the sample Microsoft Graph connector.

The sample illustrates the following concepts:

- script creating the Entra (Azure AD) app registration using the Microsoft Graph CLI
- create external connection including URL to item resolver to track activity when users share external links
- create external connection schema
- parse local markdown files to get their content and front matter metadata
- ingest content with initial activities
- visualize the external content in search results using a custom Adaptive Card
- extend Microsoft Graph Python SDK with a middleware to wait for a long-running operation to complete
- extend Microsoft Graph Python SDK with a debug middleware to show information about outgoing requests and incoming responses
- use Dev Proxy mocks to simulate creation of the external connection and its schema

## Help

We do not support samples, but this community is always willing to help, and we want to improve these samples. We use GitHub to track issues, which makes it easy for  community members to volunteer their time and help resolve issues.

You can try looking at [issues related to this sample](https://github.com/pnp/graph-connectors-samples/issues?q=label%3A%22sample%3A%python-markdown%22) to see if anybody else is having the same issues.

If you encounter any issues using this sample, [create a new issue](https://github.com/pnp/graph-connectors-samples/issues/new).

Finally, if you have an idea for improvement, [make a suggestion](https://github.com/pnp/graph-connectors-samples/issues/new).

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

![](https://m365-visitor-stats.azurewebsites.net/SamplesGallery/pnp-graph-connector-python-markdown)
