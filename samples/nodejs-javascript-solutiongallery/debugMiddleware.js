export function DebugMiddleware() {
  this.nextMiddleware = undefined;

  const getHeaders = (headers) => {
    const h = {};
    for (var header of headers.entries()) {
      h[header[0]] = header[1];
    }
    return h;
  };

  return {
    execute: async (context) => {
      console.debug('');
      console.debug(`Request: ${context.request}`);
      console.debug(JSON.stringify(context.options, null, 2));

      await this.nextMiddleware.execute(context);

      const resp = context.response.clone();

      const headers = getHeaders(resp.headers);
      console.debug('');
      console.debug('Response headers:');
      console.debug(JSON.stringify(headers, null, 2));
      if (headers.hasOwnProperty('content-type') &&
        headers['content-type'].startsWith('application/json') &&
        resp.body) {
        console.debug('');
        console.debug('Response body:');
        console.debug(JSON.stringify(await resp.json(), null, 2));
      }
    },
    setNext: (next) => {
      this.nextMiddleware = next;
    }
  }
}