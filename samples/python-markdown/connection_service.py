from connection_configuration import external_connection, schema
from graph_service import graph_client

async def _create_connection():
  print("Creating connection...", end="")
  await graph_client.external.connections.post(external_connection)
  print("DONE")

async def _create_schema():
  print("Creating schema...")
  await graph_client.external.connections.by_external_connection_id(external_connection.id).schema.patch(schema) # type: ignore
  print("DONE")

async def create_connection():
  await _create_connection()
  await _create_schema()