# -*- coding: utf-8 -*-
"""Discord announcement publisher"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from pyaddict import JDict
from aiohttp import web
import discord

from integration.discord.server import AnnouncementChannelType
from integration.discord.persona import Personas, PersonaSender
from integration.discord.util import base64_image_to_discord
from integration.manager import IntegrationManager
from common.constants import STAIR_GREEN
from db.db import Database
from db.datamodels.announcement import AnnouncementType
from webserver.msal_auth import get_username


class Announcer:
    """Announcement publisher"""

    def __init__(self, db: Database, integration: IntegrationManager):
        self._db = db
        self._integration = integration

    async def publish_announcement(  # pylint: disable=too-many-locals
        self, request: web.Request
    ) -> web.Response:
        """Publish an announcement to Discord."""
        data = JDict(await request.json())
        author = await get_username(request)
        server = data.ensureCast("server", int)
        announcement_id = data.ensureCast("id", int)
        image = data.optionalGet("image", str)
        announcement_type = data.ensureCast("type", str)
        persona = Personas.from_name(data.optionalGet("persona", str))

        announcement = self._db.get_announcement(announcement_id)

        if not announcement:
            return web.json_response({"error": "Announcement not found"}, status=404)
        if announcement_type not in AnnouncementType:
            return web.json_response(
                {
                    "error": f"Unknown announcement type {announcement_type}",
                    "valid": list(AnnouncementType),
                },
                status=400,
            )

        discord_servers = self._integration.discord.servers

        if server not in discord_servers:
            return web.json_response(
                {
                    "error": f"Unknown server {server}",
                    "valid": list(discord_servers.keys()),
                },
                status=400,
            )

        channel_type = AnnouncementChannelType.from_announcement_type(
            AnnouncementType(announcement_type)
        )
        discord_channel = channel_type.get(discord_servers[server].guild)

        if not discord_channel:
            return web.json_response(
                {
                    "error": f"Channel not found for server {server} and type {announcement_type}"
                },
                status=404,
            )

        role = channel_type.get_role(discord_servers[server].guild)

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
            embed.set_footer(text=f"by {author}")

        file = base64_image_to_discord(image)

        await PersonaSender(discord_channel, persona).send(
            message=role.mention,
            embeds=[embed_de, embed_en],
            file=file,
            publish=True,
        )
        return web.Response()
