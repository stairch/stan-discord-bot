# -*- coding: utf-8 -*-
"""Data model for announcements"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any, Callable
import time
from dataclasses import dataclass, field
from datetime import time as dtime

from pyaddict import JDict
from pyaddict.schema import Object, String, Integer, Array

from common.publish_data import PublishData
from integration.discord.persona import Personas
from .announcement import AnnouncementScope, AnnouncementType, Announcement


@dataclass
class AnnouncementSchedule:  # pylint: disable=too-many-instance-attributes
    """Data model for an announcement"""

    announcement: Announcement
    scope: AnnouncementScope
    type: AnnouncementType
    time: dtime
    days: list[int]
    server: int
    persona: str
    id: int = field(default_factory=lambda: int(time.time()))

    @classmethod
    def deserialise(
        cls, obj: Any, announcement_id: int
    ) -> tuple[AnnouncementSchedule | None, str | None]:
        """Deserialise the object"""
        dtime()
        schema = Object(
            {
                "scope": String(),
                "type": String(),
                "server": Integer().coerce(),
                "persona": String(),
                "time": String(),
                "days": Array(Integer()),
                "id": Integer().optional(),
            }
        ).withAdditionalProperties()
        validate = schema.validate(obj)
        if validate:
            data = JDict(validate.unwrap()).chain()
            return (
                cls(
                    announcement=Announcement.empty(announcement_id),
                    scope=AnnouncementScope(data.ensure("scope", str)),
                    type=AnnouncementType(data.ensure("type", str)),
                    persona=data.ensure("persona", str),
                    server=data.ensure("server", int),
                    time=dtime.fromisoformat(data.ensure("time", str)),
                    days=data.ensure("days", list),
                    id=data.ensure("id", int, int(time.time())),
                ),
                None,
            )
        return None, str(validate.error)

    def serialise(self) -> dict[str, Any]:
        """Serialise the object"""
        return {
            "scope": self.scope,
            "type": self.type,
            "server": str(self.server),
            "persona": self.persona,
            "time": self.time.isoformat(),
            "days": self.days,
            "id": self.id,
        }

    @classmethod
    def from_db(
        cls, value: Any, announcement_fetcher: Callable[[int], Announcement | None]
    ) -> AnnouncementSchedule | None:
        """from db representation"""
        data = JDict(value)
        announcement = announcement_fetcher(data.ensure("FK_announcement_id", int))
        if not announcement:
            return None
        return cls(
            announcement=announcement,
            scope=AnnouncementScope(data.ensure("scope", str)),
            type=AnnouncementType(data.ensure("type", str)),
            persona=data.ensure("persona", str),
            server=data.ensure("server", int),
            time=dtime.fromisoformat(data.ensure("time", str)),
            days=[int(x) for x in data.ensure("days", str).split(",")],
            id=data.ensure("id", int),
        )

    def to_db(self) -> Any:
        """to db representation"""
        return {
            "FK_announcement_id": self.announcement.id,
            "scope": self.scope.value,
            "type": self.type.value,
            "server": self.server,
            "persona": self.persona,
            "time": self.time.isoformat(),
            "days": ",".join([str(x) for x in self.days]),
            "id": self.id,
        }

    def as_publish_data(self) -> PublishData:
        """as publishable announcement data"""
        return PublishData(
            self.scope,
            self.type,
            Personas.from_name(self.persona),
            self.server,
            None,
            self.announcement.id,
            None,
        )