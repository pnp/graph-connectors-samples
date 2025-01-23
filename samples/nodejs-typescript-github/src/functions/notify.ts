import { app, HttpRequest, HttpResponseInit, InvocationContext } from "@azure/functions";
import { IssuesEditedEvent, IssuesEvent, IssuesOpenedEvent, PingEvent, PullRequestEvent, PushEvent } from "@octokit/webhooks-types";
import { GitHubEventHandler } from "../modules/githubEventHandlers";

export async function notify(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    try {
        context.log(`Http function processed request for url "${request.url}"`);

        // Determine which webhook event was received
        const webhookEvent = request.headers.get('x-github-event');
        context.log(`Received webhook event: ${webhookEvent}`);

        // Parse the request body
        const body = await request.json();

        // Get event handler
        const handler = GitHubEventHandler.getInstance();

        // Handle the event
        switch (webhookEvent) {
            case 'ping':
                return await handler.handlePingEvent(body as PingEvent);
            case 'issues':
                return await handler.handleIssuesEvent(body as IssuesEvent);
            case 'push':
                return await handler.handlePushEvent(body as PushEvent);
            case 'pull_request':
                return await handler.handlePullRequestEvent(body as PullRequestEvent);
            default:
                return { status: 400, body: `Unsupported webhook event: ${webhookEvent}` };
        }
    } catch (error) {
        return {
            status: 500,
            jsonBody: {
                message: "An error occurred while processing the webhook event.",
                error: error
            }
        };
    }
};

app.http('notify', {
    methods: ['POST'],
    authLevel: 'anonymous',
    handler: notify
});
