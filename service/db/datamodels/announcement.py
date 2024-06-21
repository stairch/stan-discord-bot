# -*- coding: utf-8 -*-
"""Data model for announcements"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from enum import StrEnum
from typing import Any
import time
from dataclasses import dataclass, field
from datetime import datetime

from pyaddict import JDict
from pyaddict.schema import Object, String, Integer


class AnnouncementScope(StrEnum):
    """The scope of the announcement"""

    DISCORD = "discord"
    TELEGRAM = "telegram"


class AnnouncementType(StrEnum):
    """The state of the user / server member"""

    STAIR = "stair"
    NON_STAIR = "non-stair"
    SERVER_INFO = "server"
    TEST = "test"
    CANTEEN_MENU = "canteen-menu"


@dataclass
class Announcement:
    """Data model for an announcement"""

    title: str
    message_en: str
    message_de: str
    last_author: str = ""
    last_modified: int = field(default_factory=lambda: int(time.time()))
    id: int = field(default_factory=lambda: int(time.time()))

    @classmethod
    def deserialise(
        cls, obj: Any, author: str
    ) -> tuple[Announcement | None, str | None]:
        """Deserialise the object"""
        schema = Object(
            {
                "title": String(),
                "message": Object(
                    {
                        "en": String(),
                        "de": String(),
                    }
                ),
                "id": Integer().optional(),
            }
        ).withAdditionalProperties()
        validate = schema.validate(obj)
        if validate:
            data = JDict(validate.unwrap()).chain()
            print(data)
            return (
                cls(
                    title=data.assertGet("title", str),
                    message_en=data.assertGet("message.en", str),
                    message_de=data.assertGet("message.de", str),
                    last_author=author,
                    id=data.ensure("id", int, int(time.time())),
                ),
                None,
            )
        return None, str(validate.error)

    def summary(self) -> dict[str, Any]:
        """Get a summary of the announcement"""
        return {
            "title": self.title,
            "id": self.id,
            "author": self.last_author,
            "lastModified": datetime.fromtimestamp(self.last_modified).isoformat()
            if self.last_modified
            else None,
        }

    def serialise(self) -> dict[str, Any]:
        """Serialise the object"""
        return {
            "title": self.title,
            "message": {
                "en": self.message_en,
                "de": self.message_de,
            },
            "author": self.last_author,
            "lastModified": datetime.fromtimestamp(self.last_modified).isoformat()
            if self.last_modified
            else None,
            "id": self.id,
        }
