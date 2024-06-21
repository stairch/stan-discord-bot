# -*- coding: utf-8 -*-
"""Data model for a degree programme."""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any
from dataclasses import dataclass

from pyaddict import JDict, JList
from pyaddict.schema import Array, Object, String


@dataclass
class DegreeProgramme:
    """Data model for a degree programme."""

    id: str
    category: str
    role: str
    colour: str
    channel: str

    @classmethod
    def deserialise(cls, obj: Any) -> tuple[list[DegreeProgramme] | None, str | None]:
        """Deserialise the object"""
        schema = Array(
            Object(
                {
                    "id": String(),
                    "category": String(),
                    "role": String(),
                    "colour": String(),
                    "channel": String(),
                }
            )
        )
        validate = schema.validate(obj)
        if validate:
            data = JList(validate.unwrap()).iterator().ensureCast(JDict)
            items = []
            for item in data:
                items.append(
                    cls(
                        id=item.assertGet("id", str),
                        category=item.assertGet("category", str),
                        role=item.assertGet("role", str),
                        colour=item.assertGet("colour", str),
                        channel=item.assertGet("channel", str),
                    )
                )
            return items, None
        return None, str(validate.error)
