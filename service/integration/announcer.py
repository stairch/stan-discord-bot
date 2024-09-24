# -*- coding: utf-8 -*-
"""announcer"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from aiohttp import web

from common.publish_data import PublishData
from db.db import Database
from db.datamodels.announcement import AnnouncementScope
from .iannouncer import IAnnouncer
from .discord.announcer import Announcer as DiscordAnnouncer
from .discord.stan import Stan as DiscordStan
from .telegram.announcer import Announcer as TelegramAnnouncer
from .telegram.stan import Stan as TelegramStan


class Announcer(IAnnouncer):
    """announcer supporting all announcements"""

    def __init__(
        self,
        discord_stan: DiscordStan,
        telegram_stan: TelegramStan,
    ) -> None:
        self._announcers: dict[AnnouncementScope, IAnnouncer] = {
            AnnouncementScope.DISCORD: DiscordAnnouncer(Database(), discord_stan),
            AnnouncementScope.TELEGRAM: TelegramAnnouncer(Database(), telegram_stan),
        }

    async def publish_announcement(self, data: PublishData) -> web.Response:
        """make an announcement"""
        announcer = self._announcers.get(data.scope)
        if not announcer:
            return web.json_response(
                {"error": f"Announcement scope {data.scope} not supported"},
                status=400,
            )
        return await announcer.publish_announcement(data)
