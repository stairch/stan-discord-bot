# -*- coding: utf-8 -*-
"""The main module for the STAIR Discord Bot 'Stan'."""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging
import asyncio

import uvloop
from aiohttp import web

from webserver.server import WebServer


def main():
    """Main entry point for the bot."""
    logging.basicConfig(level=logging.DEBUG)
    logger = logging.getLogger(__name__)
    logging.getLogger("discord").setLevel(logging.INFO)
    logging.getLogger("httpcore").setLevel(logging.INFO)
    logging.getLogger("telegram").setLevel(logging.INFO)
    logger.info("Starting Stan...")

    asyncio.set_event_loop_policy(uvloop.EventLoopPolicy())

    app = web.Application(client_max_size=10 * 1024**3)  # 10 MB
    WebServer(app)

    web.run_app(app, port=8080)

    logger.info("Exiting...")


if __name__ == "__main__":
    main()
