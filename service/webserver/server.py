# -*- coding: utf-8 -*-
"""Server"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio

from aiohttp import web

from stan import Stan
from .db_import import DbImportHandler
from .announcement import AnnouncementHandler


class WebServer:
    """The main entry point for the web server."""

    __slots__ = ("_db_import_handler", "_announcement_handler", "_stan")

    def __init__(self, app: web.Application) -> None:
        self._stan = Stan()
        self._db_import_handler = DbImportHandler(app, self._stan)
        self._announcement_handler = AnnouncementHandler(app, self._stan)
        app.on_startup.append(self._on_startup)

    async def _on_startup(self, _: web.Application) -> None:
        asyncio.create_task(self._stan.start())
