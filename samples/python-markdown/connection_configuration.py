from msgraph.generated.models.external_connectors.activity_settings import ActivitySettings
from msgraph.generated.models.external_connectors.external_connection import ExternalConnection
from msgraph.generated.models.external_connectors.item_id_resolver import ItemIdResolver
from msgraph.generated.models.external_connectors.url_match_info import UrlMatchInfo
from msgraph.generated.models.external_connectors.schema import Schema
from msgraph.generated.models.external_connectors.property_ import Property_
from msgraph.generated.models.external_connectors.property_type import PropertyType
from msgraph.generated.models.external_connectors.label import Label

user_id = "9da37739-ad63-42aa-b0c2-06f7b43e3e9e"

external_connection = ExternalConnection(
    id="waldekblogpython",
    name="Waldek Mastykarz (blog); Python",
    description="Tips and best practices for building applications on Microsoft 365 by Waldek Mastykarz - Microsoft 365 Cloud Developer Advocate",
    activity_settings=ActivitySettings(
        url_to_item_resolvers=[
            ItemIdResolver(
                odata_type="#microsoft.graph.externalConnectors.itemIdResolver",
                priority=1,
                item_id="{slug}",
                url_match_info=UrlMatchInfo(
                    base_urls=[
                        "https://blog.mastykarz.nl"
                    ],
                    url_pattern="/(?<slug>[^/]+)"
                )
            )
        ]
    )
)

schema = Schema(
  base_type="microsoft.graph.externalItem",
  properties=[
    Property_(
        name="title",
        type=PropertyType.String,
        is_queryable=True,
        is_searchable=True,
        is_retrievable=True,
        labels=[
            Label.Title
        ]
    ),
    Property_(
        name="excerpt",
        type=PropertyType.String,
        is_queryable=True,
        is_searchable=True,
        is_retrievable=True
    ),
    Property_(
        name="imageUrl",
        type=PropertyType.String,
        is_retrievable=True
    ),
    Property_(
        name="url",
        type=PropertyType.String,
        is_retrievable=True,
        labels=[
            Label.Url
        ]
    ),
    Property_(
        name="date",
        type=PropertyType.DateTime,
        is_queryable=True,
        is_retrievable=True,
        is_refinable=True,
        labels=[
            Label.LastModifiedDateTime
        ]
    ),
    Property_(
        name="tags",
        type=PropertyType.StringCollection,
        is_queryable=True,
        is_retrievable=True,
        is_refinable=True
    )
  ]
)