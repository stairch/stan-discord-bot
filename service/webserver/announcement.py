# -*- coding: utf-8 -*-
"""Announcement handler module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import io
import base64

from aiohttp import web
from pyaddict import JDict
import discord

from integration.discord.server import AnnouncementChannelType
from common.constants import STAIR_GREEN
from db.datamodels.announcement import Announcement, AnnouncementType
from .base_handler import BaseHandler
from .msal_auth import authenticated


class AnnouncementHandler(BaseHandler):
    """Handler for announcements"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_get("/api/announcements", self._get_announcements)
        app.router.add_get("/api/announcements/types", self._types)
        app.router.add_get("/api/announcements/discord/servers", self._discord_servers)
        app.router.add_post("/api/announcements", self._create_announcement)
        app.router.add_get("/api/announcements/{id}", self._get_announcement)
        app.router.add_put("/api/announcements/{id}", self._update_announcement)
        app.router.add_delete("/api/announcements/{id}", self._delete_announcement)
        app.router.add_post(
            "/api/announcements/{id}/publish", self._publish_announcement
        )

    @authenticated
    async def _get_announcements(self, _: web.Request) -> web.Response:
        """Get all announcements"""
        announcements = self._db.get_announcements()
        return web.json_response([x.summary() for x in announcements])

    @authenticated
    async def _get_announcement(self, request: web.Request) -> web.Response:
        """Get a single announcement"""
        id_ = int(request.match_info["id"])
        announcement = self._db.get_announcement(id_)
        if announcement is None:
            return web.json_response({"error": "Announcement not found"}, status=404)
        return web.json_response(announcement.serialise())

    @authenticated
    async def _create_announcement(self, request: web.Request) -> web.Response:
        """Create a new announcement"""
        data = JDict(await request.json())
        announcement, error = Announcement.deserialise(data)
        if announcement is None:
            return web.json_response({"error": error}, status=400)
        announcement = self._db.create_announcement(announcement)
        return web.json_response(announcement.serialise())

    @authenticated
    async def _update_announcement(self, request: web.Request) -> web.Response:
        """Update an announcement"""
        data = JDict(await request.json())
        id_ = int(request.match_info["id"])
        announcement, error = Announcement.deserialise(data)
        if announcement is None:
            return web.json_response({"error": error}, status=400)
        announcement.id = id_
        self._db.update_announcement(announcement)
        return web.json_response(announcement.serialise())

    @authenticated
    async def _delete_announcement(self, request: web.Request) -> web.Response:
        """Delete an announcement"""
        id_ = int(request.match_info["id"])
        self._db.delete_announcement(id_)
        return web.Response()

    @authenticated
    async def _publish_announcement(self, request: web.Request) -> web.Response:  # pylint: disable=too-many-locals
        """Announce a message to a channel"""
        data = JDict(await request.json())
        server = data.ensureCast("server", int)
        announcement_id = data.ensureCast("id", int)
        image = data.optionalGet("image", str)
        announcement_type = data.ensureCast("type", str)

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

        discord_servers = self._integration.stan.servers

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

        if image:
            file = discord.File(
                io.BytesIO(base64.b64decode(image)), filename="image.png"
            )
            msg = await discord_channel.send(
                f"{role.mention}", embeds=[embed_de, embed_en], file=file
            )
        else:
            msg = await discord_channel.send(
                f"{role.mention}", embeds=[embed_de, embed_en]
            )
        try:
            await msg.publish()
        except discord.errors.Forbidden as e:
            return web.json_response(
                {"error": f"Failed to publish message: {e}"}, status=206
            )
        return web.Response()

    @authenticated
    async def _types(self, _: web.Request) -> web.Response:
        """Get all announcement channels"""
        return web.json_response([x.value for x in AnnouncementType])

    @authenticated
    async def _discord_servers(self, _: web.Request) -> web.Response:
        """Get all announcement servers"""
        servers = self._integration.stan.servers
        return web.json_response([x.serialise() for x in servers.values()])
