# -*- coding: utf-8 -*-
"""Server"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio

from aiohttp import web
from aiohttp_session import setup
from aiohttp_session.cookie_storage import EncryptedCookieStorage

from stan import Stan
from .db_import import DbImportHandler
from .announcement import AnnouncementHandler
from .foodstoffi_menu_trigger import FoodstoffMenuTrigger
from .msal_auth import MsalAuth


class WebServer:
    """The main entry point for the web server."""

    __slots__ = (
        "_db_import_handler",
        "_announcement_handler",
        "_foodstoffi_menu_trigger",
        "_msal",
        "_stan",
    )

    def __init__(self, app: web.Application) -> None:
        self._stan = Stan()
        self._db_import_handler = DbImportHandler(app, self._stan)
        self._announcement_handler = AnnouncementHandler(app, self._stan)
        self._foodstoffi_menu_trigger = FoodstoffMenuTrigger(app, self._stan)
        self._msal = MsalAuth(app, self._stan)
        setup(app, EncryptedCookieStorage(b"Thirty  two  length  bytes  key."))
        app.on_startup.append(self._on_startup)

    async def _on_startup(self, _: web.Application) -> None:
        asyncio.create_task(self._stan.start())
