# -*- coding: utf-8 -*-
"""For scheduling coroutines"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Callable, Awaitable
from datetime import datetime, time
import asyncio


# run daily at
class AioSchedule:
    """Asyncio scheduler for scheduling a coroutine"""

    @staticmethod
    def _time_to_seconds(t: time) -> int:
        return t.hour * 3600 + t.minute * 60 + t.second

    @classmethod
    def _diff(cls, t1: time, t2: time) -> int:
        return cls._time_to_seconds(t1) - cls._time_to_seconds(t2)

    @classmethod
    def run_daily_at(cls, at: time, func: Callable[[], Awaitable[None]]) -> None:
        """Run a coroutine daily at a specific time"""

        async def _implement() -> None:
            while True:
                time_until = cls._diff(at, datetime.now().time())
                if time_until <= 0:
                    time_until += 24 * 3600
                await asyncio.sleep(time_until)
                await func()

        asyncio.create_task(_implement())
