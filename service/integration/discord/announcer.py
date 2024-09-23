# -*- coding: utf-8 -*-
"""Discord announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from aiohttp import web
import discord

from integration.discord.persona import PersonaSender
from integration.discord.util import base64_image_to_discord
from integration.iannouncer import IAnnouncer
from integration.manager import IntegrationManager
from common.constants import STAIR_GREEN
from common.publish_data import PublishData
from db.db import Database


class Announcer(IAnnouncer):
    """Announcement publisher"""

    def __init__(self, db: Database, integration: IntegrationManager):
        self._db = db
        self._integration = integration

    async def publish_announcement(  # pylint: disable=too-many-locals
        self, data: PublishData
    ) -> web.Response:
        """Publish an announcement to Discord."""
        announcement = self._db.get_announcement(data.announcement_id)

        if not announcement:
            return web.json_response({"error": "Announcement not found"}, status=404)

        discord_servers = self._integration.discord.servers

        if data.server not in discord_servers:
            return web.json_response(
                {
                    "error": f"Unknown server {data.server}",
                    "valid": list(discord_servers.keys()),
                },
                status=400,
            )

        server = discord_servers[data.server]
        discord_channel = server.get_announcement_channel(data.type)

        if not discord_channel:
            return web.json_response(
                {
                    "error": f"Channel not found for server {data.server} and type {data.type}"
                },
                status=404,
            )

        role = server.get_announcement_role(data.type)

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
        return web.Response()
