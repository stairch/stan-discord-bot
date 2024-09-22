# -*- coding: utf-8 -*-
"""Base announcer"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from abc import ABC, abstractmethod

from aiohttp import web

from common.publish_data import PublishData


class IAnnouncer(ABC):
    """announcer interface"""

    @abstractmethod
    async def publish_announcement(self, data: PublishData) -> web.Response:
        """publishes an announcement"""
