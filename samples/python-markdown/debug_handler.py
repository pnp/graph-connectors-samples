from kiota_http.middleware import BaseMiddleware
import httpx

class DebugHandler(BaseMiddleware):

    async def send(
        self, request: httpx.Request, transport: httpx.AsyncBaseTransport
    ) -> httpx.Response:
        print()
        print(f"{request.method} {request.url}")
        for key, value in request.headers.items():
            print(f"{key}: {value}")
        if request.content:
            print()
            print("Request body:")
            print(request.content.decode())

        response: httpx.Response = await super().send(request, transport)

        print()
        print(f"Response: {response.status_code} {response.reason_phrase}")
        print("Response headers:")
        for key, value in response.headers.items():
            print(f"{key}: {value}")

        print()
        print("Response body:")
        response_content = await response.aread()
        print(f"Response content: {response_content.decode()}")

        return response