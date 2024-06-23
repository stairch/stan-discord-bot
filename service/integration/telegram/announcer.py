# -*- coding: utf-8 -*-
"""Telegram announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from pyaddict import JDict
from aiohttp import web


from db.db import Database
from integration.manager import IntegrationManager


class Announcer:
    """Announcement publisher"""

    def __init__(self, db: Database, integration: IntegrationManager):
        self._db = db
        self._integration = integration

    async def publish_announcement(self, request: web.Request) -> web.Response:
        """Publish an announcement to Discord."""
        data = JDict(await request.json())
        server = data.ensureCast("server", int)
        image = data.optionalGet("image", str)
        announcement_id = data.ensureCast("id", int)

        announcement = self._db.get_announcement(announcement_id)

        if not announcement:
            return web.json_response({"error": "Announcement not found"}, status=404)

        chats = self._integration.telegram.chats

        if server not in chats:
            return web.json_response(
                {
                    "error": f"Unknown server {server}",
                    "valid": list(chats.keys()),
                },
                status=400,
            )

        success = await self._integration.telegram.send_announcement(
            announcement, server, image
        )
        if not success:
            return web.json_response(
                {"error": "Failed to send announcement"}, status=500
            )
        return web.Response()
