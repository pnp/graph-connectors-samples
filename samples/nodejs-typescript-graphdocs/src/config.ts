import { ExternalConnectors } from '@microsoft/microsoft-graph-types';

export const config = {
  connection: {
    id: 'msgraphdocs',
    name: 'Microsoft Graph documentation',
    description: 'Documentation for Microsoft Graph API which explains what Microsoft Graph is and how to use it.',
    activitySettings: {
      urlToItemResolvers: [
        {
          urlMatchInfo: {
            baseUrls: [
              'https://learn.microsoft.com'
            ],
            urlPattern: '/[^/]+/graph/auth/(?<slug>[^/]+)'
          },
          itemId: 'auth__{slug}',
          priority: 1
        } as ExternalConnectors.ItemIdResolver,
        {
          urlMatchInfo: {
            baseUrls: [
              'https://learn.microsoft.com'
            ],
            urlPattern: '/[^/]+/graph/sdks/(?<slug>[^/]+)'
          },
          itemId: 'sdks__{slug}',
          priority: 2
        } as ExternalConnectors.ItemIdResolver,
        {
          urlMatchInfo: {
            baseUrls: [
              'https://learn.microsoft.com'
            ],
            urlPattern: '/[^/]+/graph/(?<slug>[^/]+)'
          },
          itemId: '{slug}',
          priority: 3
        } as ExternalConnectors.ItemIdResolver
      ]
    },
    searchSettings: {
      searchResultTemplates: [
        {
          id: 'msgraphdocs',
          priority: 1,
          layout: {}
        }
      ]
    },
    // https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema: {
      baseType: 'microsoft.graph.externalItem',
      properties: [
        {
          name: 'title',
          type: 'string',
          isQueryable: true,
          isSearchable: true,
          isRetrievable: true,
          labels: [
            'title'
          ]
        },
        {
          name: 'description',
          type: 'string',
          isQueryable: true,
          isSearchable: true,
          isRetrievable: true
        },
        {
          name: 'url',
          type: 'string',
          isRetrievable: true,
          labels: [
            'url'
          ]
        },
        {
          name: 'iconUrl',
          type: 'string',
          isRetrievable: true,
          labels: [
            'iconUrl'
          ]
        }
      ]
    }
  } as ExternalConnectors.ExternalConnection
};