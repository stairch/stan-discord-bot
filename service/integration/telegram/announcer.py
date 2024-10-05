# -*- coding: utf-8 -*-
"""Telegram announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"


from common.publish_data import PublishData
from common.result import Result
from integration.iannouncer import IAnnouncer
from db.db import Database

from .stan import Stan as TelegramStan


class Announcer(IAnnouncer):
    """Announcement publisher"""

    def __init__(self, db: Database, stan: TelegramStan):
        self._db = db
        self._telegram = stan

    async def publish_announcement(self, data: PublishData) -> Result[None]:
        """Publish an announcement to Discord."""
        announcement = self._db.get_announcement(data.announcement_id)

        if not announcement:
            return Result.err("Announcement not found", status=404)

        chats = self._telegram.chats

        if data.server not in chats:
            return Result.err(
                "Server not found",
                status=404,
            )

        return await self._telegram.send_announcement(
            announcement, data.server, data.image
        )
