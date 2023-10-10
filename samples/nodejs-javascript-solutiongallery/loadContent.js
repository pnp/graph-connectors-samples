import fs from 'fs';
import logSymbols from 'log-symbols';
import ora from 'ora';
import yargs from 'yargs';

import { config } from './config.js';
import { client } from './graphClient.js';

function getLastCrawledSampleDate() {
  let lastCrawledSampleDate = new Date(0);
  try {
    const lastCrawledSampleDateStr = fs.readFileSync("latestChange.txt", "utf8");
    lastCrawledSampleDate = new Date(lastCrawledSampleDateStr);
    if (isNaN(lastCrawledSampleDate)) {
      lastCrawledSampleDate = new Date(0);
    }
  } catch (e) {
    // ignore
  }
  return lastCrawledSampleDate;
}

async function extract({ fromCache = false, sinceDate = new Date(0) }) {
  const samples = [];

  if (fromCache) {
    console.log(`Loading from cache, including samples since ${sinceDate}...`);
    const cacheString = fs.readFileSync("cache.json", "utf8");
    const cache = JSON.parse(cacheString);
    samples.push(...cache.filter((sample) => new Date(sample.updateDateTime) > sinceDate));
  } else {
    console.log(`Loading from API, including samples since ${sinceDate}...`);

    const pagination = {
      size: 50,
      index: 1,
    };

    const api = "https://m365-galleries.azurewebsites.net/Samples/searchSamples";
    const payload = {
      sort: {
        field: "updateDateTime",
        descending: true,
      },
      filter: {
        search: "",
        productId: [],
        authorId: "",
        categoryId: "",
        featuredOnly: false,
        metadata: [],
      },
      pagination,
    };

    let numSamplesRetrieved = 0;
    do {
      console.log(`Retrieving page ${pagination.index}...`);
      numSamplesRetrieved = 0;

      const response = await fetch(api, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify(payload),
      });
      const data = await response.json();

      if (data.items.length > 0) {
        const samplesToInclude = data.items.filter((sample) => new Date(sample.updateDateTime) > sinceDate);
        samples.push(...samplesToInclude);
        numSamplesRetrieved = samplesToInclude.length;
      }

      console.log(`  ${numSamplesRetrieved} samples retrieved`);
      pagination.index++;
    } while (numSamplesRetrieved > 0);

    // cache the results
    fs.writeFileSync("cache.json", JSON.stringify(samples, null, 2));
  }

  console.log(`Extracted ${samples.length} samples`);

  return samples;
}

function transform(samples) {
  return samples.map((sample) => {
    // Date must be in the ISO 8601 format
    const createdDateTime = new Date(sample.creationDateTime).toISOString();
    const lastModifiedDateTime = new Date(sample.updateDateTime).toISOString();
    const imageUrl = sample.thumbnails.length > 0 ? sample.thumbnails[0].url : "";
    return {
      id: sample.sampleId,
      properties: {
        title: sample.title,
        description: sample.shortDescription,
        "authors@odata.type": "Collection(String)",
        authors: sample.authors.map((author) => {
          const displayName = author.displayName;
          const pictureUrl = author.pictureUrl;
          return { name: displayName, pictureUrl: pictureUrl };
        }),
        /*  authors: sample.authors.map(author => author.displayName), */
        "authorsPictures@odata.type": "Collection(String)",
        /*   authorsPictures: sample.authors.map(author => author.pictureUrl), */
        authorsPictures: [],
        imageUrl,
        iconUrl:
          "https://raw.githubusercontent.com/pnp/media/master/pnp-logos-generics/png/teal/300w/pnp-samples-teal-300.png",
        url: `https://adoption.microsoft.com/sample-solution-gallery/sample/${sample.sampleId}/`,
        createdDateTime,
        lastModifiedDateTime,
        "products@odata.type": "Collection(String)",
        /*   products: sample.products, */
        products: sample.products.map((product) => {
          return { productName: product };
        }),
        "metadata@odata.type": "Collection(String)",
        metadata: sample.metadata.map((m) => `${m.key}=${m.value}`),
      },
      content: {
        value: sample.shortDescription,
        type: "text",
      },
      acl: [
        {
          accessType: "grant",
          type: "everyone",
          value: "everyone",
        },
      ],
    };
  });
}

function pushItem({ crawlInfo, id, sample, callback, errors }) {
  client
    .api(`/external/connections/${id}/items/${sample.id}`)
    .header("content-type", "application/json")
    .put(sample)
    .then(() => {
      const sampleDate = new Date(sample.properties.lastModifiedDateTime);
      if (sampleDate > crawlInfo.latestChange) {
        crawlInfo.latestChange = sampleDate;
      }
    })
    .catch((error) => {
      errors.push({
        sample,
        error,
      });
    })
    .finally(() => callback());
}

async function load(samples) {
  const spinner = ora("Loading content...").start();
  const { id } = config.connection;

  const queue = [...samples];
  const errors = [];
  const maxConnections = 10;
  let numSamples = samples.length;

  let activeRequests = 0;
  let completedRequests = 0;
  const crawlInfo = {
    latestChange: new Date(0),
  };

  const printStatus = () => {
    spinner.text = `Total: ${numSamples} | Processed: ${completedRequests} (${(
      (completedRequests / numSamples) *
      100
    ).toFixed(1)}%) | Queued: ${queue.length} | Errors: ${errors.length} | Active: ${activeRequests}...`;
  };

  while (queue.length > 0) {
    printStatus();

    if (activeRequests >= maxConnections) {
      await new Promise((resolve) => setTimeout(resolve, 100));
      continue;
    }

    const sample = queue.shift();
    // we could end up here if there are no more samples to process
    // but we still have active requests
    if (!sample) {
      continue;
    }

    activeRequests++;

    // use callback to allow parallel requests
    // move uploading to a separate function to have a closure
    // which is needed to capture information about the sample that
    // failed
    pushItem({
      crawlInfo,
      id,
      sample,
      callback: () => {
        activeRequests--;
        completedRequests++;
      },
      errors,
    });
  }

  // wait for all requests to complete
  while (activeRequests > 0) {
    await new Promise((resolve) => setTimeout(resolve, 100));
    printStatus();
  }

  fs.writeFileSync("latestChange.txt", crawlInfo.latestChange.toISOString());

  if (errors.length > 0) {
    const errorFileName = `errors-${new Date().toISOString().replace(/[^\d\w]/g, "-")}.json`;
    fs.writeFileSync(errorFileName, JSON.stringify(errors, null, 2));
    spinner.warn(`Completed with ${errors.length} errors. See ${errorFileName} for details.`);
  } else {
    spinner.succeed();
  }
}

async function main() {
  const lastCrawledSampleDate = getLastCrawledSampleDate();

  let fromCache = yargs(process.argv).argv.fromCache;
  if (fromCache === undefined) {
    fromCache = true;
  }
  const samples = await extract({ fromCache, sinceDate: lastCrawledSampleDate });
  if (samples.length === 0) {
    console.log(`${logSymbols.success} No new samples to load`);
    return;
  }

  const transformed = transform(samples);
  await load(transformed);
}

main();
