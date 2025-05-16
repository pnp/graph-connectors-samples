import { Config } from "../models/Config";
import { Item } from "../models/Item";

/**
 * Fetches repairs from Repairs Hub API.
 * @param config - The configuration object.
 * @param fetchUrl - The URL to fetch repairs from.
 * @returns A promise that resolves to the response.
 */
async function fetchRepairs(config: Config, fetchUrl: string): Promise<Response> {
  // Use the access token from the config if available
  // to authenticate the request to the GitHub API if using a private repo
  // or to avoid rate limiting
  const headers: HeadersInit = {};
  if (config.connector.accessToken) {
    headers["Authorization"] = `Bearer ${config.connector.accessToken}`;
  }
  const response = await fetch(fetchUrl, {
    headers,
  });
  if (!response.ok) {
    throw new Error(`Failed to fetch repairs: ${response.statusText}`);
  }
  return response;
}

/**
 * Gets the URL for the next page of results from the response headers.
 * Parse the link header to find the next page URL.
 * @param response - The response object from the fetch request.
 * @returns The URL for the next page of results, or null if there are no more pages.
 */
function getNextPageUrl(response: Response): string | null {
  const linkHeader = response.headers.get("link");
  if (!linkHeader) {
    return null;
  }
  const links = linkHeader.split(", ");
  for (const link of links) {
    const match = link.match(/<([^>]+)>; rel="next"/);
    if (match) {
      return match[1];
    }
  }
  return null;
}

async function mapItemsFromResponse(response: Response): Promise<Item[]> {
  const repairs: any[] = await response.json();

  const mappedItems = repairs.map<Item>((repair) => {
    return {
      id: repair.id as string,
      title: repair.title, 
      description: repair.description, 
      assignedTo: repair.assignedTo, 
      date: repair.date, 
      iconUrl: repair.image
    };
  });
  return mappedItems;
}

// [Customization point]
// If you need additional logic to get all items from the repairs hub, you can add it here
// This function is used to get all items from the repairs hub API.
/**
 * Get all items for all repairs sequentially, yielding data as soon as it's available.
 * @param config - The configuration object containing connector details
 * @param pageSize - Number of items per page. Defaults to 100.
 */
export async function* getAllItemsFromAPI(
  config: Config,
  pageSize: number = 100
): AsyncGenerator<Item> {
    config.context.log(`Fetching repairs from CRM: RepairsHub`);

    // Url to fetch issues for the first page
    let fetchPageUrl = `https://repairshub.azurewebsites.net/repairs`;
    while (fetchPageUrl) {
      const response = await fetchRepairs(config, fetchPageUrl);
      const issues = await mapItemsFromResponse(response);
      for (const item of issues) {
        yield item;
      }

      // If there are no more pages, null is returned to break the loop
      fetchPageUrl = getNextPageUrl(response);
    }
}
