export const config = {
  // 3-32 characters
  id: 'waldekblogcli',
  name: 'Waldek Mastykarz (blog)',
  description: 'Tips and best practices for building applications on Microsoft 365 by Waldek Mastykarz - Microsoft 365 Cloud Developer Advocate',
  activitySettings: {
    urlToItemResolvers: [
      {
        '@odata.type': '#microsoft.graph.externalConnectors.itemIdResolver',
        urlMatchInfo: {
          baseUrls: [
            'https://blog.mastykarz.nl'
          ],
          urlPattern: '/(?<slug>[^/]+)'
        },
        itemId: '{slug}',
        priority: 1
      }
    ]
  },
  schema: {
    "baseType": "microsoft.graph.externalItem",
    "properties": [
      {
        "name": "title",
        "type": "String",
        "isQueryable": true,
        "isSearchable": true,
        "isRetrievable": true,
        "labels": [
          "title"
        ]
      },
      {
        "name": "excerpt",
        "type": "String",
        "isQueryable": true,
        "isSearchable": true,
        "isRetrievable": true
      },
      {
        "name": "imageUrl",
        "type": "String",
        "isRetrievable": true
      },
      {
        "name": "url",
        "type": "String",
        "isRetrievable": true,
        "labels": [
          "url"
        ]
      },
      {
        "name": "date",
        "type": "DateTime",
        "isQueryable": true,
        "isRetrievable": true,
        "isRefinable": true,
        "labels": [
          "lastModifiedDateTime"
        ]
      },
      {
        "name": "tags",
        "type": "StringCollection",
        "isQueryable": true,
        "isRetrievable": true,
        "isRefinable": true
      }
    ]
  }
};