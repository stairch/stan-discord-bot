# -*- coding: utf-8 -*-
"""Data model for a verified user."""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from enum import StrEnum
from dataclasses import dataclass, field


class UserState(StrEnum):
    """The state of the user / server member"""

    GUEST = "Unverified"
    STUDENT = "Student"
    GRADUATE = "Graduate"


@dataclass
class VerifiedUser:
    """Data model for a verified user"""

    discord_id: int
    email: str
    state: UserState
    id: int = field(default=0)
