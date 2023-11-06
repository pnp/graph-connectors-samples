export const config = {
  connection: {
    // 3-32 characters
    id: 'ibmdb2lob',
    name: 'IBM DB2 - LOB - SALES',
    description: 'Sample solutions that demonstrate use Microsoft 365 extensibility capabilities. to Search Line of Business applications ',
    searchSettings: {
      searchResultTemplates: [
        {
          id: 'samplesolgallery',
          priority: 1,
          layout: {}
        }
      ]
    },
    searchSettings: {
      searchResultTemplates: [
        {
          id: 'ibmdb2lob',
          priority: 1,
          layout: {}
        }
      ]
    },
    // https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema: [
      {
        name: 'custcode',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'custname',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'orders',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'orderdates',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'ordertotals',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },
      {
        name: 'orderstatus',
        type: 'StringCollection',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'
      },

      {
        name: 'country',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'city',
        type: 'String',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'state',
        type: 'String',
        isRetrievable: 'true',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'email',
        type: 'String',
        isRetrievable: 'true',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'phone',
        type: 'String',
        isRetrievable: 'true',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
      {
        name: 'address',
        type: 'String',
        isRetrievable: 'true',
        isQueryable: 'true',
        isSearchable: 'true',
        isRetrievable: 'true'

      },
    ]
  }
};
