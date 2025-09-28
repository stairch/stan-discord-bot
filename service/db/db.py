# -*- coding: utf-8 -*-
"""Database module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging
import os
from dataclasses import asdict
from typing import Callable
from datetime import datetime

import dataset  # type: ignore
import sqlalchemy  # type: ignore

from common.singleton import Singleton
from .datamodels.hslu_student import HsluStudent
from .datamodels.verified_user import VerifiedUser, UserState
from .datamodels.announcement import Announcement
from .datamodels.schedule import AnnouncementSchedule
from .datamodels.degree_programme import DegreeProgramme

VERIFIED_USERS_TABLE = "Verified_Users"
HSLU_STUDENTS_TABLE = "HSLU_Students"
ANNOUNCEMENTS_TABLE = "Announcements"
SCHEDULES_TABLE = "Announcement_Schedules"
DEGREE_PROGRAMMES_TABLE = "Degree_Programmes"

DB_USERNAME = os.getenv("POSTGRES_USER")
DB_PASSWORD = os.getenv("POSTGRES_PASSWORD")
DB_DATABASE = os.getenv("POSTGRES_DB")
DEV_MODE = os.getenv("DEV_MODE", "false").lower() == "true"
DB_HOST = "postgres:5432"
DB_PROTOCOL = "postgresql"


class Database(metaclass=Singleton):  # pylint: disable=too-many-instance-attributes
    """Wraps the database using dataset & our custom data models"""

    def __init__(self) -> None:
        self._db = dataset.connect(
            f"{DB_PROTOCOL}://{DB_USERNAME}:{DB_PASSWORD}@{DB_HOST}/{DB_DATABASE}"
        )
        self._logger = logging.getLogger("Database")

        self._users_table: dataset.Table = self._db[VERIFIED_USERS_TABLE]
        self._hslu_students_table: dataset.Table = self._db[HSLU_STUDENTS_TABLE]
        self._announcements_table: dataset.Table = self._db[ANNOUNCEMENTS_TABLE]
        self._schedules_table: dataset.Table = self._db[SCHEDULES_TABLE]
        self._degree_programmes_table: dataset.Table = self._db[DEGREE_PROGRAMMES_TABLE]

        self._on_schedule_change: set[Callable[[list[AnnouncementSchedule]], None]] = (
            set()
        )

        self._logger.warning("database connected, DEV_MODE=%s", DEV_MODE)
        if DEV_MODE:
            self._users_table.delete()

    @property
    def on_schedule_change(self) -> set[Callable[[list[AnnouncementSchedule]], None]]:
        """on schedula change callbacks"""
        return self._on_schedule_change

    def _fire_on_schedule_change(self) -> None:
        for callback in self._on_schedule_change:
            callback(self.all_schedules())

    def all_students(self) -> list[HsluStudent]:
        """Get all students from the database."""
        return [HsluStudent(**x) for x in self._hslu_students_table.all()]

    def update_students(
        self, students: list[HsluStudent]
    ) -> tuple[list[VerifiedUser], list[VerifiedUser]]:
        """
        Update the students in the database. This will also update the state of
        the users in the database.

        Returns a list of new graduates.
        """
        current_members = self.all_verified()
        previous_students = [x for x in current_members if x.state == UserState.STUDENT]
        previous_graduates = [
            x for x in current_members if x.state == UserState.GRADUATE
        ]

        self._hslu_students_table.drop()
        # make unique
        students = list({student.email: student for student in students}.values())
        self._hslu_students_table.insert_many([asdict(student) for student in students])
        # all that were previously students should now be graduates
        new_graduates = []
        new_students = []

        for student in previous_students:
            if any(x.email == student.email for x in students):
                continue
            self._logger.debug("%s is now a graduate", student)
            self._users_table.update(
                {"state": UserState.GRADUATE, "email": student.email},
                ["email"],
            )
            new_graduates.append(student)

        for student in previous_graduates:
            if not any(x.email == student.email for x in students):
                continue
            self._logger.debug("%s is now a student again", student)
            self._users_table.update(
                {"state": UserState.STUDENT, "email": student.email},
                ["email"],
            )
            new_students.append(student)

        return new_graduates, new_students

    def student_by_email(self, email: str) -> HsluStudent | None:
        """Search for a student by email. Returns None if not found."""
        result = self._hslu_students_table.find_one(email=email)
        if result:
            return HsluStudent(**result)
        return None

    def get_member(
        self, discord_id: int | None = None, email: str | None = None
    ) -> VerifiedUser | None:
        """Get a verified user by their Discord ID, returning None if not yet verified."""
        result: dict | None = None
        if discord_id is not None:
            result = self._users_table.find_one(discord_id=discord_id)
        if result is None and email is not None:
            result = self._users_table.find_one(email=email)
        if result:
            return VerifiedUser(**result)
        return None

    def all_verified(self) -> list[VerifiedUser]:
        """Get all verified users from the database."""
        return [VerifiedUser(**x) for x in self._users_table.all()]

    def verify_member(self, discord_id: int, email: str) -> None:
        """Verify a member by their Discord ID and email."""
        self._users_table.insert(
            asdict(
                VerifiedUser(
                    discord_id=discord_id, email=email, state=UserState.STUDENT
                )
            )
        )

    def get_announcements(self) -> list[Announcement]:
        """Get all announcements from the database."""
        all_rows = self._announcements_table.find(order_by=["-last_modified"])
        all_announcements = [Announcement(**x) for x in all_rows]
        self._logger.info(
            "items: %s", [(x.title, x.last_modified) for x in all_announcements]
        )
        return all_announcements

    def search_announcements(
        self,
        query: str | None = None,
        author: str | None = None,
        time_range: tuple[datetime | None, datetime | None] | None = None,
    ) -> list[Announcement]:
        """Search announcements by query, author, and time range."""
        result: list[Announcement] = []

        sql_query: list[str] = []

        if author:
            sql_query.append(f"last_author = '{author}'")
        if time_range:
            if time_range[0]:
                sql_query.append(f"last_modified >= {time_range[0].timestamp()}")
            if time_range[1]:
                sql_query.append(f"last_modified <= {time_range[1].timestamp()}")
        if query:
            sql_query.append(f"title ILIKE '%{query}%'")

        if sql_query:
            text_statement = " AND ".join(sql_query)
            select = self._announcements_table.table.select(
                sqlalchemy.text(text_statement)
            ).order_by(sqlalchemy.desc("last_modified"))
            for row in self._db.query(select):
                if not row:
                    continue
                item = Announcement(**row)
                result.append(item)
            return result
        return self.get_announcements()

    def get_announcement(self, id_: int) -> Announcement | None:
        """Get an announcement by its ID."""
        result = self._announcements_table.find_one(id=id_)
        if result:
            return Announcement(**result)
        return None

    def create_announcement(self, announcement: Announcement) -> Announcement:
        """Create an announcement."""
        id_ = self._announcements_table.insert(asdict(announcement))
        announcement.id = id_
        return announcement

    def delete_announcement(self, id_: int) -> None:
        """Delete an announcement by its ID."""
        self._announcements_table.delete(id=id_)

    def update_announcement(self, announcement: Announcement) -> None:
        """Update an announcement."""
        self._announcements_table.update(asdict(announcement), ["id"])

    def all_schedules(self) -> list[AnnouncementSchedule]:
        """Get all schedules."""
        result: list[AnnouncementSchedule] = []
        for row in self._schedules_table.all():
            if not row:
                continue
            item = AnnouncementSchedule.from_db(row, self.get_announcement)
            if not item:
                continue
            result.append(item)
        return result

    def get_schedules(self, announcement_id: int) -> list[AnnouncementSchedule]:
        """Get schedules by announcement ID."""
        result: list[AnnouncementSchedule] = []
        for row in self._schedules_table.find(FK_announcement_id=announcement_id):
            if not row:
                continue
            item = AnnouncementSchedule.from_db(row, self.get_announcement)
            if not item:
                continue
            result.append(item)
        return result

    def delete_schedule(self, id_: int) -> None:
        """Get an announcement by its ID."""
        self._schedules_table.delete(id=id_)
        self._fire_on_schedule_change()

    def upsert_schedule(self, schedule: AnnouncementSchedule) -> None:
        """Get an announcement by its ID."""
        self._schedules_table.upsert(schedule.to_db(), ["id"])
        self._fire_on_schedule_change()

    def delete_schedules_except(
        self, announcement_id: int, schedule_ids: list[int]
    ) -> None:
        """Deletes all of an announcement's schedules except for the ids specified"""
        schedules = self._schedules_table.find(FK_announcement_id=announcement_id)
        for schedule in schedules:
            if not schedule or schedule.get("id") in schedule_ids:
                continue
            self._schedules_table.delete(id=schedule.get("id"))
        self._fire_on_schedule_change()

    def get_degree_programmes(self) -> list[DegreeProgramme]:
        """Get all degree programmes from the database."""
        return [DegreeProgramme(**x) for x in self._degree_programmes_table.all()]

    def update_degree_programmes(
        self, degree_programmes: list[DegreeProgramme]
    ) -> None:
        """Update the degree programmes in the database."""
        self._degree_programmes_table.drop()
        self._degree_programmes_table = self._db.create_table(
            DEGREE_PROGRAMMES_TABLE,
            primary_id="id",
            primary_type=self._db.types.text,
            primary_increment=False,
        )
        self._degree_programmes_table.insert_many(
            [asdict(degree_programme) for degree_programme in degree_programmes]
        )
