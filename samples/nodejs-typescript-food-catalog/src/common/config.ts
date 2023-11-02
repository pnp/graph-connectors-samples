import { resultLayout } from "./resultLayout";

export const config = {
  aadAppTenantId: process.env.AAD_APP_TENANT_ID,
  aadAppClientId: process.env.AAD_APP_CLIENT_ID,
  aadAppClientSecret: process.env.AAD_APP_CLIENT_SECRET,
  storageAccountConnectionString: process.env.AzureWebJobsStorage,
  notificationEndpoint: process.env.NOTIFICATION_ENDPOINT,
  connector: {
    // 3-32 characters
    id: 'foodstore',
    name: 'Food store',
    description: 'Contains information about food products, such as their name, ingredients, nutrition facts, and allergens.',
    activitySettings: {
      urlToItemResolvers: [
        {
          '@odata.type': '#microsoft.graph.externalConnectors.itemIdResolver',
          urlMatchInfo: {
            baseUrls: [
              'https://world.openfoodfacts.org'
            ],
            urlPattern: '/product/(?<productId>[^/]+)/.*'
          },
          itemId: '{productId}',
          priority: 1
        }
      ]
    },
    searchSettings: {
      searchResultTemplates: [
        {
          id: 'foodstore',
          priority: 1,
          layout: resultLayout
        }
      ]
    },
    // https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema: [
      {
        name: 'name',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true',
        labels: [
          'title'
        ]
      },
      {
        name: 'categories',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'ecoscore',
        type: 'String',
        isQueryable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'imageUrl',
        type: 'String',
        isRetrievable: 'true'
      },
      {
        name: 'ingredients',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'nutriscore',
        type: 'String',
        isQueryable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'traces',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'url',
        type: 'String',
        isRetrievable: 'true',
        labels: [
          'url'
        ]
      }
    ]
  }
};