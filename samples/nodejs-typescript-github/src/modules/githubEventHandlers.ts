import { HttpResponseInit } from "@azure/functions";
import { IssuesEvent, PingEvent, PullRequestEvent, PushEvent } from "@octokit/webhooks-types";
import { Graph } from "./graph";
import { ExternalConnectors } from "@microsoft/microsoft-graph-types";

export class GitHubEventHandler {
    private static instance: GitHubEventHandler | undefined;
    private graph: Graph | undefined;

    private constructor() { }

    public static getInstance(): GitHubEventHandler {
        if (!GitHubEventHandler.instance) {
            GitHubEventHandler.instance = new GitHubEventHandler();
        }
        return GitHubEventHandler.instance;
    }

    private async getGraphClient(): Promise<Graph> {
        if (!this.graph) {
            this.graph = new Graph();
        }
        return this.graph;
    }

    /**
     * Creates an ExternalItem from an IssuesEvent
     * @param event The IssuesEvent
     * @returns The ExternalItem
     */
    private createExternalItemFromIssueEvent(event: IssuesEvent): ExternalConnectors.ExternalItem {
        return {
            id: event.issue.id.toString(),
            acl: [
                {
                    accessType: "grant",
                    type: "everyone",
                    value: "everyone"
                }
            ],
            properties: {
                title: `Issue: ${event.issue.title} #${event.issue.number}`,
                url: event.issue.html_url,
                iconUrl: event.repository.owner.avatar_url,
                createdBy: event.issue.user.login,
                createdDateTime: event.issue.created_at,
                lastModifiedDateTime: event.issue.updated_at,
                // Custom properties
                number: event.issue.number.toString(),
                repository: event.repository.full_name,
                status: event.issue.state,
                statusImageUrl: event.issue.state === "open" ? "https://img.shields.io/badge/Status-Open-brightgreen" : "https://img.shields.io/badge/Status-Closed-red",
            },
            content: {
                type: "text",
                value: event.issue.body
            }
        };
    }

    /**
     * Creates an ExternalItem from a PushEvent
     * @param event The PushEvent
     * @returns The ExternalItem
     */
    private createExternalItemFromPushEvent(event: PushEvent): ExternalConnectors.ExternalItem | undefined {
        if (!event.head_commit) {
            return undefined;
        }

        return {
            id: event.head_commit.id.toString(),
            acl: [
                {
                    accessType: "grant",
                    type: "everyone",
                    value: "everyone"
                }
            ],
            properties: {
                title: `Push to ${event.repository.full_name}: ${event.head_commit.message}`,
                url: event.head_commit.url,
                iconUrl: event.repository.owner.avatar_url,
                createdBy: event.pusher.name,
                createdDateTime: event.head_commit.timestamp,
                lastModifiedDateTime: event.head_commit.timestamp,
                // Custom properties
                repository: event.repository.full_name,
                status: "push",
                statusImageUrl: "https://img.shields.io/badge/Status-Pushed-blue"
            },
            content: {
                type: "text",
                value: `Pushed ${event.commits.length} commits to ${event.repository.full_name}. The following files have been changed: ${event.commits.flatMap(c => c.modified).filter((value, index, self) => self.indexOf(value) === index).join(", ")}`
            }
        };
    }

    /**
     * Creates an ExternalItem from a PullRequestEvent
     * @param event The PullRequestEvent
     * @returns The ExternalItem
     */
    private createExternalItemFromPullRequestEvent(event: PullRequestEvent): ExternalConnectors.ExternalItem {
        return {
            id: event.pull_request.id.toString(),
            acl: [
                {
                    accessType: "grant",
                    type: "everyone",
                    value: "everyone"
                }
            ],
            properties: {
                title: `Pull Request: ${event.pull_request.title} #${event.pull_request.number}`,
                url: event.pull_request.html_url,
                iconUrl: event.repository.owner.avatar_url,
                createdBy: event.pull_request.user.login,
                createdDateTime: event.pull_request.created_at,
                lastModifiedDateTime: event.pull_request.updated_at,
                // Custom properties
                number: event.pull_request.number.toString(),
                repository: event.repository.full_name,
                status: event.pull_request.state,
                statusImageUrl: event.pull_request.state === "open"
                    ? "https://img.shields.io/badge/Status-Open-brightgreen"
                    : event.pull_request.merged
                        ? "https://img.shields.io/badge/Status-Merged-purple"
                        : "https://img.shields.io/badge/Status-Closed-red",
            },
            content: {
                type: "text",
                value: event.pull_request.body
            }
        };
    }

    /**
     * Handles the IssuesEvent, which is a generic handler for all issue events
     * @param event The IssuesEvent
     * @returns The response to return to the caller
     */
    public async handleIssuesEvent(event: IssuesEvent): Promise<HttpResponseInit> {
        try {
            const graph = await this.getGraphClient();
            const externalItem = this.createExternalItemFromIssueEvent(event);
            await graph.putExternalItem(externalItem);

            return { body: `Issue ${event.action}: ${event.issue.title} #${event.issue.number} on ${event.repository.full_name}` };
        } catch (error) {
            console.error(`An error occurred while processing the issue event: ${error}`);
            return { status: 500, body: `An error occurred while processing the issue event: ${error}` };
        }
    }

    /**
     * Handles the PushEvent, which is a generic handler for all push events
     * @param event The PushEvent
     * @returns The response to return to the caller
     */
    public async handlePushEvent(event: PushEvent): Promise<HttpResponseInit> {
        try {
            // Create an ExternalItem from the PushEvent
            const externalItem = this.createExternalItemFromPushEvent(event);
            if (externalItem) {
                const graph = await this.getGraphClient();
                await graph.putExternalItem(externalItem);
            } else {
                console.warn(`Received push event without head_commit, ignoring`);
            }
            return { body: `Push event on ${event.repository.full_name}` };
        } catch (error) {
            console.error(`An error occurred while processing the push event: ${error}`);
            return { status: 500, body: `An error occurred while processing the push event: ${error}` };
        }
    }

    /** Handles the PullRequestEvent
     * @param event The PullRequestEvent
     * @returns The response to return to the caller
     */
    public async handlePullRequestEvent(event: PullRequestEvent): Promise<HttpResponseInit> {
        try {
            const graph = await this.getGraphClient();
            const externalItem = this.createExternalItemFromPullRequestEvent(event);
            await graph.putExternalItem(externalItem);

            return { body: `Pull Request ${event.action}: ${event.pull_request.title} #${event.pull_request.number} on ${event.repository.full_name}` };
        } catch (error) {
            console.error(`An error occurred while processing the pull request event: ${error}`);
            return { status: 500, body: `An error occurred while processing the pull request event: ${error}` };
        }
    }

    /**
     * Handles the PingEvent, which is a generic handler for all ping events
     * @param event The PingEvent
     * @returns The response to return to the caller
     */
    public async handlePingEvent(event: PingEvent): Promise<HttpResponseInit> {
        console.log(`Received ping event ID ${event.hook_id}`);
        // Do nothing other than return a success response
        return { body: `Pong!` };
    }
}