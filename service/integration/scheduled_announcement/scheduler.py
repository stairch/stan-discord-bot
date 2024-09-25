# -*- coding: utf-8 -*-
"""Announcement Scheduler"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio
from typing import Callable, Awaitable, Any
import logging

from common.aioschedule import AioSchedule
from common.publish_data import PublishData
from db.db import Database
from db.datamodels.schedule import AnnouncementSchedule


class Scheduler:
    """Announcement Scheduler"""

    def __init__(
        self, on_announcement: Callable[[PublishData], Awaitable[Any | None]]
    ) -> None:
        self._logger = logging.getLogger("AnnouncementScheduler")
        self._db: Database = Database()
        self._on_announcement = on_announcement
        self._tasks: dict[int, asyncio.Task | None] = {}
        self._db.on_schedule_change.add(self._load)

    def start(self) -> None:
        """on startup"""
        self._load()

    def _create_task(self, schedule: AnnouncementSchedule) -> None:
        if schedule.id in self._tasks:
            return

        async def _implement() -> None:
            self._logger.debug(
                "sending scheduled announcement %s (scope: %s)",
                schedule.announcement.title,
                type(schedule.scope),
            )
            res = await self._on_announcement(schedule.as_publish_data())
            self._logger.debug(
                "scheduled announcement %s sent (%s)", schedule.announcement.title, res
            )

        self._tasks[schedule.id] = AioSchedule.run_weekly_at(
            schedule.time, schedule.days, _implement
        )
        self._logger.info("created scheduled task %s", schedule.announcement.title)

    def _load(self, new_schedules: list[AnnouncementSchedule] | None = None) -> None:
        for task in self._tasks.values():
            if not task:
                continue
            task.cancel()
        self._tasks.clear()
        self._logger.info("cancelled all scheduled announcements...")
        schedules = new_schedules or self._db.all_schedules()
        for schedule in schedules:
            self._create_task(schedule)
        self._logger.info("scheduled all announcements")
