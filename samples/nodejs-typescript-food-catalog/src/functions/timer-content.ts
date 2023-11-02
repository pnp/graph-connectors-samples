import { InvocationContext, Timer, app } from "@azure/functions";
import { startCrawl } from "../common/queueClient";

app.timer('incrementalCrawl', {
  // every hour
  schedule: '0 * * * *',
  handler: async (timer: Timer, context: InvocationContext) => {
    context.log(`Enqueuing request for incremental crawl...`);
    startCrawl('incremental');
  }
});

app.timer('removeDeleted', {
  // 10 past every hour
  schedule: '10 * * * *',
  handler: async (timer: Timer, context: InvocationContext) => {
    context.log(`Enqueuing request for cleaning deleted items...`);
    startCrawl('removeDeleted');
  }
});