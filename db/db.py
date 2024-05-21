# -*- coding: utf-8 -*-
"""Database module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging
from dataclasses import asdict
import dataset  # type: ignore
from common.singleton import Singleton
from .datamodels.hslu_student import HsluStudent
from .datamodels.verified_user import VerifiedUser, UserState

VERIFIED_USERS_TABLE = "Verified_Users"
HSLU_STUDENTS_TABLE = "HSLU_Students"

DB_USERNAME = "postgres"
DB_PASSWORD = "postgres"
DB_HOST = "localhost:5432"
DB_PROTOCOL = "postgresql"


class Database(metaclass=Singleton):
    """Wraps the database using dataset & our custom data models"""

    def __init__(self) -> None:
        self._db = dataset.connect(
            f"{DB_PROTOCOL}://{DB_USERNAME}:{DB_PASSWORD}@{DB_HOST}/"
        )
        self._logger = logging.getLogger("Database")

        self._users_table: dataset.Table = self._db[VERIFIED_USERS_TABLE]
        self._hslu_students_table: dataset.Table = self._db[HSLU_STUDENTS_TABLE]

        # self._users_table.delete() # for development

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

    def get_member(self, discord_id) -> VerifiedUser | None:
        """Get a verified user by their Discord ID, returning None if not yet verified."""
        result = self._users_table.find_one(discord_id=discord_id)
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