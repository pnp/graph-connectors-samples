import asyncio
from args import args
from connection_service import create_connection
from content_service import load_content

if args.subcommand == "create-connection":
    asyncio.run(create_connection())
elif args.subcommand == "load-content":
    asyncio.run(load_content())
