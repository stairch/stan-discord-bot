# -*- coding: utf-8 -*-
"""For scheduling coroutines"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Callable, Awaitable
from datetime import datetime, time, timedelta
import asyncio
import logging

ONE_DAY_IN_SECONDS = 24 * 3600


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
                    time_until += ONE_DAY_IN_SECONDS
                await asyncio.sleep(time_until)
                await func()

        asyncio.create_task(_implement())

    @classmethod
    def run_weekly_at(
        cls, at: time, weekdays: list[int], func: Callable[[], Awaitable[None]]
    ) -> asyncio.Task | None:
        """
        Run a coroutine weekly at a specific time on specific weekdays

        :param weekdays: list of weekdays where 0 = Monday, 6 = Sunday
        """
        if len(weekdays) == 0:
            return None

        if any(not (0 <= x <= 6) for x in weekdays):
            raise ValueError("Weekdays must be between 0 (Monday) and 6 (Sunday)")

        weekdays.sort()

        async def _implement() -> None:
            while True:
                now = datetime.now()
                time_until = cls._diff(at, now.time())

                next_target = now.weekday()

                if time_until <= 0:
                    # was today
                    time_until += ONE_DAY_IN_SECONDS
                    next_target = (next_target + 1) % 7

                if next_target not in weekdays:
                    next_weekday_to_run = next(
                        (x for x in weekdays if x >= next_target), weekdays[0] + 7
                    )
                    time_until += ONE_DAY_IN_SECONDS * (
                        next_weekday_to_run - next_target
                    )

                logging.getLogger().debug(
                    "Next run at %s in %d seconds",
                    datetime.now() + timedelta(seconds=time_until),
                    time_until
                )

                await asyncio.sleep(time_until)
                await func()

        return asyncio.create_task(_implement())
