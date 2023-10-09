import configparser
from azure.identity import ClientSecretCredential
from httpx import AsyncClient, Timeout
from kiota_authentication_azure.azure_identity_authentication_provider import AzureIdentityAuthenticationProvider
from kiota_http.kiota_client_factory import DEFAULT_CONNECTION_TIMEOUT, DEFAULT_REQUEST_TIMEOUT
from msgraph import GraphServiceClient, GraphRequestAdapter
from msgraph_core import GraphClientFactory

from args import args
from complete_job_with_delay_handler import CompleteJobWithDelayHandler
from debug_handler import DebugHandler

_config = configparser.ConfigParser()
_config.read("config.ini")

_tenant_id = _config["AZURE"]["TENANT_ID"]
_client_id = _config["AZURE"]["CLIENT_ID"]
_client_secret = _config["AZURE"]["CLIENT_SECRET"]

_credential = ClientSecretCredential(_tenant_id, _client_id, _client_secret)
_auth_provider = AzureIdentityAuthenticationProvider(_credential)

timeout = Timeout(DEFAULT_REQUEST_TIMEOUT, connect=DEFAULT_CONNECTION_TIMEOUT)
if args.with_proxy:
    _proxies = {
        "http://": "http://0.0.0.0:8000",
        "https://": "http://0.0.0.0:8000",
    }
    http_client = AsyncClient(proxies=_proxies, verify=False, timeout=timeout, http2=True) # type: ignore
else:
    http_client = AsyncClient(timeout=timeout, http2=True)

_middleware = GraphClientFactory.get_default_middleware(None)
# _middleware.append(DebugHandler())
_middleware.insert(0, CompleteJobWithDelayHandler(60000))
http_client = GraphClientFactory.create_with_custom_middleware(_middleware, client=http_client)
_adapter = GraphRequestAdapter(_auth_provider, http_client)

graph_client = GraphServiceClient(_credential,
                                  scopes=[
                                      "https://graph.microsoft.com/.default"],
                                  request_adapter=_adapter)
