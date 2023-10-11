import copy
import httpx
import time
import logging
from kiota_http.middleware import BaseMiddleware
from kiota_abstractions.serialization.parse_node_factory_registry import ParseNodeFactoryRegistry
from msgraph.generated.models.external_connectors.connection_operation import ConnectionOperation
from msgraph.generated.models.external_connectors.connection_operation_status import ConnectionOperationStatus


logging.basicConfig()
logger = logging.getLogger(__name__)


class CompleteJobWithDelayHandler(BaseMiddleware):
    def __init__(self, delayMs: int) -> None:
        super().__init__()
        self.delayMs = delayMs

    @staticmethod
    def _create_new_request(method: str, url: str, original_request: httpx.Request) -> httpx.Request:
        new_request = httpx.Request(
            method=method,
            url=url,
            headers=original_request.headers,
            extensions=original_request.extensions)
        
        if method == "GET":
            new_request.headers["Content-Length"] = "0"

        # required by Kiota middleware
        new_request.context = original_request.context #type: ignore
        new_request.options = original_request.options #type: ignore
        return new_request

    async def send(
        self, request: httpx.Request, transport: httpx.AsyncBaseTransport
    ) -> httpx.Response:
        # since middleware modifies the request object, we need to get its value before
        # otherwise we lose context and options which leads to exceptions
        request_before = copy.deepcopy(request)

        response: httpx.Response = await super().send(request, transport)

        location = response.headers.get("Location")
        if location:
            logger.info("Location: %s", location)

            if "/operations/" not in location:
                # not a job URL we should follow
                return response
          
            logger.info("Waiting %sms before following location %s...", self.delayMs, location)
            time.sleep(self.delayMs / 1000)
            
            new_request = self._create_new_request("GET", location, request_before)
            return await self.send(new_request, transport)

        if "/operations/" not in str(request.url):
            # not a job
            return response
        
        if not response.is_success:
            logger.warning("Response is not OK")
            return response

        body_bytes = response.read()
        parse_node = ParseNodeFactoryRegistry().get_root_parse_node("application/json", body_bytes) # type: ignore
        operation: ConnectionOperation = parse_node.get_object_value(ConnectionOperation.create_from_discriminator_value(parse_node)) # type: ignore

        if operation.status == ConnectionOperationStatus.Inprogress:
            logger.info("Waiting %sms before trying again...", self.delayMs)
            time.sleep(self.delayMs / 1000)
            new_request = self._create_new_request("GET", str(request_before.url), request_before)
            return await self.send(new_request, transport)
        else:
            return response
