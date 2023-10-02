export function CompleteJobWithDelayMiddleware(delayMs) {
  this.nextMiddleware = undefined;
  this.execute = async (context) => {
    // wait for response
    await this.nextMiddleware.execute(context);

    const location = context.response.headers.get('location');
    if (location) {
      console.debug(`Location: ${location}`);

      if (location.indexOf('/operations/') < 0) {
        // not a job URL we should follow
        return;
      }

      console.log(`Waiting ${delayMs}ms before following location ${location}...`);
      await new Promise(resolve => setTimeout(resolve, delayMs));

      context.request = location;
      context.options.method = 'GET';
      context.options.body = undefined;
      await this.execute(context);
      return;
    }

    if (context.request.indexOf('/operations/') < 0) {
      // not a job
      return;
    }

    const res = context.response.clone();
    if (!res.ok) {
      console.debug('Response is not OK');
      return;
    }
    const body = await res.json();
    console.debug(`Status: ${body.status}`);
    if (body.status === 'inprogress') {
      console.debug(`Waiting ${delayMs}ms before trying again...`);
      await new Promise(resolve => setTimeout(resolve, delayMs));
      await this.execute(context);
    }
  }

  return {
    execute: async (context) => {
      return await this.execute(context);
    },
    setNext: (next) => {
      this.nextMiddleware = next;
    }
  }
}