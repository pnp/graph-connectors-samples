import { ExternalConnectors } from "@microsoft/microsoft-graph-types";
import * as resultLayout from "./resultLayout.json";

interface IExternalConnectorConfig {
    externalConnection: ExternalConnectors.ExternalConnection;
    schema: ExternalConnectors.Schema;
}

export const config: IExternalConnectorConfig = {
    externalConnection: {
        id: process.env.CONNECTION_ID,
        name: "GitHub",
        description: "This connection is used to get get information about our GitHub projects, repositories, issues and other items.",
        searchSettings: {
            searchResultTemplates: [
                {
                    id: "githubResult",
                    layout: resultLayout
                }
            ]
        },
    },
    schema: {
        baseType: "microsoft.graph.externalItem",
        properties: [
            // Standard properties
            {
                name: "title",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                labels: ["title"],
                aliases: [
                    "name",
                    "subject"
                ]
            },
            {
                name: "url",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                labels: ["url"],
                aliases: [
                    "link",
                    "href"
                ]
            },
            {
                name: "iconUrl",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                labels: ["iconUrl"],
            },
            {
                name: "createdBy",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                labels: ["createdBy"],
                aliases: [
                    "creator"
                ]
            },
            {
                name: "createdDateTime",
                type: "dateTime",
                isQueryable: true,
                isRetrievable: true,
                labels: ["createdDateTime"]
            },
            {
                name: "lastModifiedDateTime",
                type: "dateTime",
                isQueryable: true,
                isRetrievable: true,
                labels: ["lastModifiedDateTime"]
            },
            // Custom properties
            {
                name: "number",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                aliases: [
                    "issue",
                    "issueNumber",
                    "prNumber",
                    "pullRequestNumber",
                ]
            },
            {
                name: "repository",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                aliases: [
                    "repo",
                    "repositoryName",
                    "repoName"
                ]
            },
            {
                name: "status",
                type: "string",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                aliases: [
                    "state"
                ]
            },
            {
                name: "statusImageUrl",
                type: "string",
                isQueryable: false,
                isSearchable: false,
                isRetrievable: true,
            },
            {
                name: "labels",
                type: "stringCollection",
                isQueryable: true,
                isSearchable: true,
                isRetrievable: true,
                aliases: [
                    "keywords",
                    "tags"
                ]
            }
        ]
    }
}