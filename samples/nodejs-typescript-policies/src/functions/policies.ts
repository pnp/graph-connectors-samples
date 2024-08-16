import { app, Timer, InvocationContext } from "@azure/functions";
import { DeployConnections } from "../common/DeployConnection";

/**
 * This function handles the HTTP request and returns the repair information.
 *
 * @param {HttpRequest} req - The HTTP request.
 * @param {InvocationContext} context - The Azure Functions context object.
 * @returns {Promise<HttpResponseInit>} - A promise that resolves with the HTTP response containing the repair information.
 */
export async function policies(
  timer: Timer,
  context: InvocationContext
): Promise<void> {
  context.log("Starting policies function...");

  await DeployConnections();
}

app.timer("polciesApp", {
    schedule: '0 0 0 30 2 *',
    runOnStartup: true,
    handler: policies
});
