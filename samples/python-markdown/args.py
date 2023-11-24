import argparse

_parser = argparse.ArgumentParser(description="Ingest local markdown files to Microsoft 365")
_parser.add_argument("--with-proxy", action="store_true", help="Enable Dev Proxy")

_subparsers = _parser.add_subparsers(title="Command to run", dest="subcommand", required=True)
_subparsers.add_parser("create-connection", help="Creates external connection")
_subparsers.add_parser("load-content", help="Loads content into the external connection")

args = _parser.parse_args()