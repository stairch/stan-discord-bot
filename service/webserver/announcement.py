# -*- coding: utf-8 -*-
"""Announcement handler module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from datetime import datetime

from aiohttp import web
from pyaddict import JDict, JList

from integration.discord.persona import Persona
from common.publish_data import PublishData
from db.datamodels.announcement import Announcement, AnnouncementType
from db.datamodels.schedule import AnnouncementSchedule
from .base_handler import BaseHandler
from .msal_auth.auth import authenticated, get_username


class AnnouncementHandler(BaseHandler):
    """Handler for announcements"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_get("/api/announcements", self._get_announcements)
        app.router.add_get("/api/announcements/types", self._types)
        app.router.add_get("/api/announcements/discord/servers", self._discord_servers)
        app.router.add_get("/api/announcements/telegram/chats", self._telegram_chats)
        app.router.add_get("/api/announcements/personas", self._personas)
        app.router.add_post("/api/announcements", self._create_announcement)
        app.router.add_get("/api/announcements/{id}", self._get_announcement)
        app.router.add_get("/api/announcements/{id}/schedules", self._get_schedules)
        app.router.add_put("/api/announcements/{id}", self._update_announcement)
        app.router.add_put("/api/announcements/{id}/schedules", self._update_schedules)
        app.router.add_delete("/api/announcements/{id}", self._delete_announcement)
        app.router.add_post(
            "/api/announcements/{id}/publish", self._publish_announcement
        )

    @authenticated
    async def _get_announcements(self, request: web.Request) -> web.Response:
        """Get all announcements"""
        # query can contain 'query', 'author', 'start' and 'end'
        request_query = JDict(dict(request.query))
        search_query = request_query.optionalGet("query", str)
        author: str | None = None
        if request_query.optionalGet("author", str) == "me":
            author = await get_username(request)
        start_stamp = request_query.optionalGet("start", str)
        end_stamp = request_query.optionalGet("end", str)
        start = datetime.fromisoformat(start_stamp) if start_stamp else None
        end = datetime.fromisoformat(end_stamp) if end_stamp else None

        limit = request_query.ensureCast("limit", int, 20)
        offset = request_query.ensureCast("offset", int, 0)

        announcements = self._db.search_announcements(
            search_query, author, time_range=(start, end)
        )
        announcement_window = announcements[offset : offset + limit]
        return web.json_response(
            [x.summary() for x in announcement_window],
            headers={
                "X-Total-Count": str(len(announcements)),
                "X-Count": str(len(announcement_window)),
                "X-Offset": str(offset),
                "X-Limit": str(limit),
            },
        )

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
        announcement, error = Announcement.deserialise(
            data, await get_username(request)
        )
        if announcement is None:
            return web.json_response({"error": error}, status=400)
        announcement = self._db.create_announcement(announcement)
        return web.json_response(announcement.serialise())

    @authenticated
    async def _update_announcement(self, request: web.Request) -> web.Response:
        """Update an announcement"""
        data = JDict(await request.json())
        id_ = int(request.match_info["id"])
        announcement, error = Announcement.deserialise(
            data, await get_username(request)
        )
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
    async def _publish_announcement(self, request: web.Request) -> web.Response:
        """Announce a message to a channel"""
        data = await PublishData.from_request(request)
        result = await self._integration.publish_announcement(data)
        return result.as_web_response()

    @authenticated
    async def _get_schedules(self, request: web.Request) -> web.Response:
        announcement_id = int(request.match_info["id"])
        schedules = self._db.get_schedules(announcement_id)
        return web.json_response([x.serialise() for x in schedules])

    @authenticated
    async def _update_schedules(self, request: web.Request) -> web.Response:
        schedules = JList(await request.json()).iterator().ensureCast(JDict)
        announcement_id = int(request.match_info["id"])
        schedule_ids = [x.get("id", int) for x in schedules]
        self._db.delete_schedules_except(announcement_id, schedule_ids)
        for schedule in schedules:
            item, err = AnnouncementSchedule.deserialise(schedule, announcement_id)
            if err or item is None:
                return web.json_response({"error": err}, status=400)
            self._db.upsert_schedule(item)
        return web.Response()

    @authenticated
    async def _types(self, _: web.Request) -> web.Response:
        """Get all announcement channels"""
        return web.json_response(AnnouncementType.serialise())

    @authenticated
    async def _discord_servers(self, _: web.Request) -> web.Response:
        """Get all announcement servers"""
        servers = self._integration.discord.servers
        return web.json_response([x.serialise() for x in servers.values()])

    @authenticated
    async def _telegram_chats(self, _: web.Request) -> web.Response:
        """Get all announcement servers"""
        servers = self._integration.telegram.chats
        return web.json_response(
            [{"id": x.id, "name": x.title, "picture": None} for x in servers.values()]
        )

    @authenticated
    async def _personas(self, _: web.Request) -> web.Response:
        """Get all announcement servers"""
        return web.json_response(Persona.serialise())
