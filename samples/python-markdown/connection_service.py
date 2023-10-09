import json
from msgraph.generated.models.json import Json
from connection_configuration import external_connection, schema
from graph_service import graph_client

async def _create_connection():
  print("Creating connection...", end="")
  with open('resultLayout.json', 'r') as file:
    adaptive_card = file.read()
    layout = json.loads(adaptive_card)

    assert external_connection.search_settings is not None
    assert external_connection.search_settings.search_result_templates is not None

    external_connection.search_settings.search_result_templates[0].layout = Json(
      additional_data=layout
    )

  await graph_client.external.connections.post(external_connection)
  print("DONE")

async def _create_schema():
  print("Creating schema...")

  assert external_connection.id is not None
  await graph_client.external.connections.by_external_connection_id(external_connection.id).schema.patch(schema)
  
  print("DONE")

async def create_connection():
  await _create_connection()
  await _create_schema()