import { app, HttpRequest, InvocationContext } from "@azure/functions";
import { CrawlType } from "../common/ContentMessage";
import { startCrawl } from "../common/queueClient";
import { streamToJson } from "../common/utils";

interface CrawlRequest {
  crawlType?: CrawlType;
}

app.http('content', {
  methods: ['POST'],
  route: 'crawl',
  handler: async (request: HttpRequest, context: InvocationContext) => {
    const body: CrawlRequest = await streamToJson(request.body);

    if (!body.crawlType || !['full', 'incremental'].includes(body.crawlType)) {
      return {
        status: 400
      }
    }

    context.log(`Enqueuing crawl request for ${body.crawlType}...`);
    startCrawl(body.crawlType);

    return {
      status: 202
    }
  }
})