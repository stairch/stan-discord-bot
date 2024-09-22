# -*- coding: utf-8 -*-
"""Server"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio
import os

from aiohttp import web
from aiohttp_session import setup
from aiohttp_session.cookie_storage import EncryptedCookieStorage

from integration.manager import IntegrationManager
from integration.announcer import Announcer
from .db_import import DbImportHandler
from .announcement import AnnouncementHandler
from .foodstoffi_menu_trigger import FoodstoffMenuTrigger
from .msal_auth.handler import MsalAuth


class WebServer:
    """The main entry point for the web server."""

    __slots__ = (
        "_db_import_handler",
        "_announcement_handler",
        "_foodstoffi_menu_trigger",
        "_msal",
        "_integration_manager",
        "_announcer",
    )

    def __init__(self, app: web.Application) -> None:
        self._integration_manager = IntegrationManager()
        self._announcer = Announcer(self._integration_manager)
        self._db_import_handler = DbImportHandler(
            app, self._integration_manager, self._announcer
        )
        self._announcement_handler = AnnouncementHandler(
            app, self._integration_manager, self._announcer
        )
        self._foodstoffi_menu_trigger = FoodstoffMenuTrigger(
            app, self._integration_manager, self._announcer
        )
        self._msal = MsalAuth(app, self._integration_manager, self._announcer)
        setup(app, EncryptedCookieStorage(os.getenv("SESSION_SECRET", "").encode()))
        app.on_startup.append(self._on_startup)

    async def _on_startup(self, _: web.Application) -> None:
        asyncio.create_task(self._integration_manager.start())
