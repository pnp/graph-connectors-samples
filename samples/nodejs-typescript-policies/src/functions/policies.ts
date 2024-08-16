import { app, Timer, InvocationContext } from "@azure/functions";
import { DeployConnections } from "../common/DeployConnection";

/**
 * This function handles the HTTP request and returns the repair information.
 *
 * @param {HttpRequest} req - The HTTP request.
 * @param {InvocationContext} context - The Azure Functions context object.
 * @returns {Promise<HttpResponseInit>} - A promise that resolves with the HTTP response containing the repair information.
 */
export async function policies(): Promise<void> {
  // Starts the deployment of the Graph connector.
  await DeployConnections();
}

app.timer("policies", {
    schedule: '0 0 0 30 2 *',
    runOnStartup: true,
    handler: policies
});
