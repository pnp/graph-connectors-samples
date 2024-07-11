import { Context, Middleware } from '@microsoft/microsoft-graph-client';

export class CompleteJobWithDelayMiddleware implements Middleware
{
  private nextMiddleware?: Middleware;

  constructor(private delayMs: number) {
  }

  public execute = async (context: Context) => {
    // wait for response
    await this.nextMiddleware?.execute(context);

    const location = context.response?.headers.get('location');
    if (location) {
      console.debug(`Location: ${location}`);

      if (location.indexOf('/operations/') < 0) {
        // not a job URL we should follow
        return;
      }

      console.log(`Waiting ${this.delayMs}ms before following location ${location}...`);
      await new Promise(resolve => setTimeout(resolve, this.delayMs));

      context.request = location;
      if (context.options) {
        context.options.method = 'GET';
        context.options.body = undefined;
      }
      await this.execute(context);
      return;
    }

    if (context.request.toString().indexOf('/operations/') < 0) {
      // not a job
      return;
    }

    const res = context.response?.clone();
    if (!res?.ok) {
      console.debug('Response is not OK');
      return;
    }
    const body = await res.json();
    console.debug(`Status: ${body.status}`);
    if (body.status === 'inprogress') {
      console.debug(`Waiting ${this.delayMs}ms before trying again...`);
      await new Promise(resolve => setTimeout(resolve, this.delayMs));
      await this.execute(context);
    }
  }

  public setNext(next: Middleware) {
    this.nextMiddleware = next;
  }
}