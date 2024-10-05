# -*- coding: utf-8 -*-
"""Discord announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import discord

from integration.discord.persona import PersonaSender
from integration.discord.util import base64_image_to_discord
from integration.iannouncer import IAnnouncer
from common.constants import STAIR_GREEN
from common.publish_data import PublishData
from common.result import Result
from db.db import Database

from .stan import Stan as DiscordStan


class Announcer(IAnnouncer):
    """Announcement publisher"""

    def __init__(self, db: Database, stan: DiscordStan):
        self._db = db
        self._discord = stan

    async def publish_announcement(  # pylint: disable=too-many-locals
        self, data: PublishData
    ) -> Result[None]:
        """Publish an announcement to Discord."""
        announcement = self._db.get_announcement(data.announcement_id)

        if not announcement:
            return Result.err(error="Announcement not found", status=404)

        discord_servers = self._discord.servers

        if data.server not in discord_servers:
            return Result.err(
                "Server not found",
                status=404,
            )

        server = discord_servers[data.server]
        discord_channel = server.get_announcement_channel(data.announcement_type)

        if not discord_channel:
            return Result.err(
                "Channel not found",
                status=404,
            )

        role = server.get_announcement_role(data.announcement_type)

        embed_de = discord.Embed(
            title=f":flag_de: {announcement.title}",
            description=announcement.message_de,
            color=STAIR_GREEN,
        )
        embed_en = discord.Embed(
            title=f":flag_gb: {announcement.title}",
            description=announcement.message_en,
            color=STAIR_GREEN,
        )

        for embed in (embed_de, embed_en):
            if data.user:
                embed.set_footer(text=f"by {data.user}")
            else:
                embed.set_footer(text="automated announcement")

        file = base64_image_to_discord(data.image)

        await PersonaSender(discord_channel, data.persona).send(
            message=role.mention,
            embeds=[embed_de, embed_en],
            file=file,
            publish=True,
        )
        return Result.ok(value=None)
