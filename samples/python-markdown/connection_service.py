import json
import logging

from msgraph.generated.models.json import Json
from connection_configuration import external_connection, schema
from graph_service import graph_client

logging.basicConfig()
logger = logging.getLogger(__name__)


async def _create_connection():
    logger.info("Creating connection...")
    with open("resultLayout.json", "r", encoding='utf-8') as file:
        adaptive_card = file.read()
        layout = json.loads(adaptive_card)

        assert external_connection.search_settings is not None
        assert external_connection.search_settings.search_result_templates is not None

        external_connection.search_settings.search_result_templates[0].layout = Json(
            additional_data=layout
        )

    await graph_client.external.connections.post(external_connection)
    logger.info("DONE")


async def _create_schema():
    logger.info("Creating schema...")

    assert external_connection.id is not None
    await graph_client.external.connections.by_external_connection_id(
        external_connection.id
    ).schema.patch(schema)

    logger.info("DONE")


async def create_connection():
    await _create_connection()
    await _create_schema()
