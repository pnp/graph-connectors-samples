export const config = {
  connection: {
    // 3-32 characters
    id: 'samplesolutiongallery',
    name: 'Sample Solution Gallery',
    description: 'Sample solutions that demonstrate different Microsoft 365 extensibility capabilities.',
    activitySettings: {
      urlToItemResolvers: [
        {
          '@odata.type': '#microsoft.graph.externalConnectors.itemIdResolver',
          urlMatchInfo: {
            baseUrls: [
              'https://adoption.microsoft.com'
            ],
            urlPattern: '/sample-solution-gallery/sample/(?<sampleId>[^/]+)'
          },
          itemId: '{sampleId}',
          priority: 1
        }
      ]
    },
    searchSettings: {
      searchResultTemplates: [
        {
          id: 'samplesolgallery',
          priority: 1,
          layout: {}
        }
      ]
    },
    // https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema: [
      {
        name: 'title',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true',
        labels: [
          'title'
        ]
      },
      {
        name: 'description',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true',
      },
      {
        name: 'authors',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true',
        labels: [
          'authors'
        ]
      },
      {
        name: 'authorsPictures',
        type: 'StringCollection',
        isRetrievable: 'true'
      },
      {
        name: 'imageUrl',
        type: 'String',
        isRetrievable: 'true'
      },
      {
        name: 'iconUrl',
        type: 'String',
        isRetrievable: 'true',
        labels: [
          'iconUrl'
        ]
      },
      {
        name: 'url',
        type: 'String',
        isRetrievable: 'true',
        labels: [
          'url'
        ]
      },
      {
        name: 'createdDateTime',
        type: 'DateTime',
        isQueryable: 'true',
        isRetrievable: 'true',
        isRefinable: 'true',
        labels: [
          'createdDateTime'
        ]
      },
      {
        name: 'lastModifiedDateTime',
        type: 'DateTime',
        isQueryable: 'true',
        isRetrievable: 'true',
        isRefinable: 'true',
        labels: [
          'lastModifiedDateTime'
        ]
      },
      {
        name: 'products',
        type: 'StringCollection',
        isQueryable: 'true',
        isRetrievable: 'true',
        isRefinable: 'true'
      },
      {
        name: 'metadata',
        type: 'StringCollection',
        isQueryable: 'true',
        isRetrievable: 'true',
        isRefinable: 'true',
        isExactMatchRequired: 'true'
      }
    ]
  }
};
