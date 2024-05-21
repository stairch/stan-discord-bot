# -*- coding: utf-8 -*-
"""Web server base handler"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from abc import ABC, abstractmethod
from logging import Logger

from aiohttp import web

from stan import Stan
from db.db import Database


class BaseHandler(ABC):
    """Base handler for the web server"""

    __slots__ = ("_logger", "_stan", "_db")

    def __init__(self, app: web.Application, stan: Stan) -> None:
        self._logger: Logger = Logger("WebServer")
        self._stan = stan
        self._db: Database = Database()
        self._add_routes(app)

    @abstractmethod
    def _add_routes(self, app: web.Application) -> None:
        """Add routes to the application"""
