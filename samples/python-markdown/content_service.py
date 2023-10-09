import frontmatter
import os
import re
from bs4 import BeautifulSoup
from datetime import datetime
from markdown import markdown
from typing import Generator
from urllib.parse import urljoin
from msgraph.generated.models.external_connectors.access_type import AccessType
from msgraph.generated.models.external_connectors.acl import Acl
from msgraph.generated.models.external_connectors.acl_type import AclType
from msgraph.generated.models.external_connectors.external_activity import ExternalActivity
from msgraph.generated.models.external_connectors.external_activity_type import ExternalActivityType
from msgraph.generated.models.external_connectors.external_item import ExternalItem
from msgraph.generated.models.external_connectors.external_item_content import ExternalItemContent
from msgraph.generated.models.external_connectors.external_item_content_type import ExternalItemContentType
from msgraph.generated.models.external_connectors.identity import Identity
from msgraph.generated.models.external_connectors.identity_type import IdentityType
from msgraph.generated.models.external_connectors.properties import Properties

from connection_configuration import external_connection, user_id
from graph_service import graph_client

def _markdown_to_text(markdown_string):
    """ Converts a markdown string to plaintext """

    # md -> html -> text since BeautifulSoup can extract text cleanly
    html = markdown(markdown_string)

    # remove code snippets
    html = re.sub(r'<pre>(.*?)</pre>', ' ', html)
    html = re.sub(r'<code>(.*?)</code >', ' ', html)

    # extract text
    soup = BeautifulSoup(html, "html.parser")
    text = ''.join(soup.findAll(text=True))

    return text

def _extract():
  base_url = "https://blog.mastykarz.nl"
  folder_path = "content"

  for filename in os.listdir(folder_path):
    file_path = os.path.join(folder_path, filename)
    if not os.path.isfile(file_path) and (filename.endswith(".markdown") or filename.endswith(".md")):
        return
    
    with open(file_path, "r") as file:
      post = frontmatter.load(file)
      post.content = _markdown_to_text(post.content)
      post.url = urljoin(base_url, post.metadata["slug"]) # type: ignore
      post.image = urljoin(base_url, post.metadata["image"]) # type: ignore
      yield post

def _transform(content) -> Generator[ExternalItem, None, None]:
  # needed to properly format activity date
  local_time_with_timezone = datetime.now().astimezone()

  for post in content:
    # Date must be in the ISO 8601 format
    if isinstance(post.metadata["date"], str):
      post.metadata["date"] = datetime.strptime(post.metadata["date"], "%Y-%m-%d %H:%M:%S")
    
    date: str = post.metadata["date"].isoformat()
    yield ExternalItem(
      id=post.metadata["slug"],
      properties=Properties(
        additional_data={
          "title": post.metadata["title"],
          "excerpt": post.metadata["excerpt"],
          "imageUrl": post.image,
          "url": post.url,
          "date": date,
          "tags@odata.type": "Collection(String)",
          "tags": post.metadata["tags"]
        }
      ),
      content=ExternalItemContent(
        type=ExternalItemContentType.Text,
        value=post.content
      ),
      acl=[
        Acl(
          type=AclType.Everyone,
          value="everyone",
          access_type=AccessType.Grant
        )
      ],
      activities=[
        ExternalActivity(
          odata_type="#microsoft.graph.externalConnectors.externalActivity",
          type=ExternalActivityType.Created,
          start_date_time=post.metadata["date"].replace(tzinfo=local_time_with_timezone.tzinfo),
          performed_by=Identity(
            type=IdentityType.User,
            id=user_id
          )
        )
      ]
    )

async def _load(content: Generator[ExternalItem, None, None]):
  for doc in content:
    try:
      print(f"Loading {doc.id}...", end="")
      await graph_client.external.connections.by_external_connection_id(
          external_connection.id # type: ignore
        ).items.by_external_item_id(doc.id).put(doc) # type: ignore
      print("DONE")
    except Exception as e:
      print(f"Failed to load {doc.id}: {e}")
      return

async def load_content():
  content = _extract()
  transformed = _transform(content)
  await _load(transformed)
