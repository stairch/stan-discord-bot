# -*- coding: utf-8 -*-
"""Publish data datamodel"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any
from dataclasses import dataclass

from pyaddict import JDict
from aiohttp import web

from db.datamodels.announcement import AnnouncementType, AnnouncementScope
from webserver.msal_auth.auth import get_username
from integration.discord.persona import Persona


@dataclass
class PublishData:
    """publish data datamodel"""

    scope: AnnouncementScope
    type: AnnouncementType
    persona: Persona
    server: int
    image: str | None
    announcement_id: int
    user: str | None

    @classmethod
    def from_dict(cls, value: Any, username: str | None = None) -> "PublishData":
        """from dict"""
        data = JDict(value)
        announcement_type = AnnouncementType.get(data.ensureCast("type", str))
        if not announcement_type:
            raise ValueError(f"Invalid announcement type: {data.ensure('type', str)}")
        persona = Persona.get(data.ensure("persona", str))
        if not persona:
            raise ValueError(f"Invalid persona: {data.ensure('type', str)}")
        return cls(
            type=announcement_type,
            scope=AnnouncementScope(data.ensureCast("scope", str)),
            server=data.ensureCast("server", int),
            persona=persona,
            user=username,
            image=data.optionalGet("image", str),
            announcement_id=data.ensureCast("id", int),
        )

    @classmethod
    async def from_request(cls, request: web.Request) -> "PublishData":
        """from aiohttp web request"""
        try:
            user = await get_username(request)
            return cls.from_dict(await request.json(), user)
        except web.HTTPClientError:
            raise
        except Exception as e:  # pylint: disable=broad-except
            raise web.HTTPBadRequest(text=str(e))
