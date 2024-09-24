# -*- coding: utf-8 -*-
"""Telegram announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from aiohttp import web

from common.publish_data import PublishData
from integration.iannouncer import IAnnouncer
from db.db import Database

from .stan import Stan as TelegramStan


class Announcer(IAnnouncer):
    """Announcement publisher"""

    def __init__(self, db: Database, stan: TelegramStan):
        self._db = db
        self._telegram = stan

    async def publish_announcement(self, data: PublishData) -> web.Response:
        """Publish an announcement to Discord."""
        announcement = self._db.get_announcement(data.announcement_id)

        if not announcement:
            return web.json_response({"error": "Announcement not found"}, status=404)

        chats = self._telegram.chats

        if data.server not in chats:
            return web.json_response(
                {
                    "error": f"Unknown server {data.server}",
                    "valid": list(chats.keys()),
                },
                status=400,
            )

        success = await self._telegram.send_announcement(
            announcement, data.server, data.image
        )
        if not success:
            return web.json_response(
                {"error": "Failed to send announcement"}, status=500
            )
        return web.Response()
