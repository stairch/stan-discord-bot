# -*- coding: utf-8 -*-
"""Announcement handler module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import io
import base64

from aiohttp import web
from pyaddict import JDict
import discord

from guild import AnnouncementChannelType
from .base_handler import BaseHandler


class AnnouncementHandler(BaseHandler):
    """Handler for announcements"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_post("/api/announcement", self._announce)
        app.router.add_get("/api/announcement/channels", self._announcement_channels)
        app.router.add_get("/api/announcement/servers", self._announcement_servers)

    async def _announce(self, request: web.Request) -> web.Response:
        """Announce a message to a channel"""
        data = JDict(await request.json())
        channel = data.ensure("channel", str)
        server = data.ensure("server", str)
        title = data.ensure("title", str)
        message = data.ensureCast("message", JDict)
        publish = data.ensure("publish", bool)
        image = data.optionalGet("image", str)

        message_en = message.ensure("en", str)
        message_de = message.ensure("de", str)

        if channel not in AnnouncementChannelType:
            return web.json_response({"error": f"Unknown channel {channel}"}, status=400)
        if server not in self._stan.servers:
            return web.json_response({"error": f"Unknown server {server}"}, status=400)

        channel_type = AnnouncementChannelType(channel)
        discord_channel = channel_type.get(self._stan.servers[server].guild)
        role = channel_type.get_role(self._stan.servers[server].guild)

        embed_de = discord.Embed(title=f":flag_de: {title}", description=message_de, color=0x0B6A5B)
        embed_en = discord.Embed(title=f":flag_gb: {title}", description=message_en, color=0x0B6A5B)

        if image:
            file = discord.File(io.BytesIO(base64.b64decode(image)), filename="image.png")
            msg = await discord_channel.send(
                f"{role.mention}", embeds=[embed_de, embed_en], file=file
            )
        else:
            msg = await discord_channel.send(
                f"{role.mention}", embeds=[embed_de, embed_en], file=file
            )
        if publish:
            await msg.publish()
        return web.Response()

    async def _announcement_channels(self, _: web.Request) -> web.Response:
        """Get all announcement channels"""
        return web.json_response([x.value for x in AnnouncementChannelType])

    async def _announcement_servers(self, _: web.Request) -> web.Response:
        """Get all announcement servers"""
        servers = self._stan.servers
        return web.json_response([x.serialise() for x in servers.values()])
