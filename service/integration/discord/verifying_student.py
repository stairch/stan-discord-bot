# -*- coding: utf-8 -*-
"""State machine for verifying students."""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from enum import Enum
from typing import cast
import re
import asyncio
import logging
import string
import secrets

import discord

from db.db import Database
from db.datamodels.verified_user import UserState
from integration.email.client import EmailClient

GENERIC_VALIDATION_ERROR_MESSAGE = """
You must provide a valid email address ending in @stud.hslu.ch.
Please try again or contact a STAIR member.
"""


class VerificationState(Enum):
    """the state of the verification process"""

    PENDING = 0
    WAITING_FOR_EMAIL = 1
    WAITING_FOR_CODE = 2
    VERIFIED = 3
    EXPIRED = 4


class VerifyingStudent:
    """State machine for verifying students"""

    _users: dict[int, VerifyingStudent] = {}

    def __init__(self, member: discord.Member, email_client: EmailClient) -> None:
        self._member: discord.Member = member
        self._email_client: EmailClient = email_client
        self._state = VerificationState.PENDING
        self._verification_code = self._generate_verification_code()
        self._email: str | None = None
        self._db = Database()
        self._logger = logging.getLogger("verifying_student")

    @property
    def email(self) -> str:
        """The email address of the student, raises AssertionError if not yet set"""
        assert self._email is not None
        return self._email

    async def handle(self, msg: str = "") -> bool:
        """Handle a message"""
        match self._state:
            case VerificationState.PENDING:
                await self._handle_pending(msg)
                return False
            case VerificationState.WAITING_FOR_EMAIL:
                await self._handle_waiting_for_email(msg)
                return False
            case VerificationState.WAITING_FOR_CODE:
                return await self._handle_waiting_for_code(msg)
            case VerificationState.EXPIRED:
                await self._handle_expired(msg)
                return False
            case VerificationState.VERIFIED:
                await self._handle_verified(msg)
                return False

    async def _expire(self) -> None:
        await asyncio.sleep(5 * 60)
        if self._state == VerificationState.WAITING_FOR_CODE:
            self._state = VerificationState.EXPIRED

    async def _handle_pending(self, msg: str) -> None:
        self._state = VerificationState.WAITING_FOR_EMAIL

        if msg:
            await self._handle_waiting_for_email(msg)
            return

        await self._member.send(
            "Welcome to the server! Please send me your email to verify!"
        )

    async def _handle_waiting_for_email(self, msg: str) -> None:
        email = re.search(r"[a-zA-Z0-9_.+-]+@stud.hslu.ch", msg)

        if not email:
            await self._member.send(GENERIC_VALIDATION_ERROR_MESSAGE)
            return
        db: Database = Database()
        student = db.student_by_email(email.group().lower())

        if not student:
            await self._member.send(GENERIC_VALIDATION_ERROR_MESSAGE)
            return

        email_norm = email.group().lower()

        if db.get_member(discord_id=self._member.id, email=email_norm):
            await self._member.send(GENERIC_VALIDATION_ERROR_MESSAGE)
            return

        self._email = email_norm
        await self._send_verification_code(
            f"Nice to meet you, {student.first_name.split(' ')[0]}! I sent you an email with a verification code. Please provide it here."  # pylint: disable=line-too-long
        )

    async def _send_verification_code(self, msg: str) -> None:
        await self._email_client.send_email(
            "STAIR Discord Verification Code",
            f"Your verification code is {self._verification_code}",
            self.email,
        )
        await self._member.send(msg)
        self._state = VerificationState.WAITING_FOR_CODE
        asyncio.create_task(self._expire())

    async def _handle_waiting_for_code(self, msg: str) -> bool:
        if msg != self._verification_code:
            await self._member.send("Invalid verification code. Please try again.")
            return False

        self._state = VerificationState.VERIFIED
        try:
            Database().verify_member(self._member.id, self.email)
        except Exception as e:  # pylint: disable=broad-except
            self._logger.exception(e)
            await self._member.send(
                "Oops, that didn't work. Please try again or contact a STAIR member."
            )
            return False

        await self._member.send("Awesome! You have been verified!")
        return True

    async def _handle_expired(self, _: str) -> None:
        self._verification_code = self._generate_verification_code()
        await self._send_verification_code(
            "Your verification code has expired. I sent you a new one."
        )

    async def _handle_verified(self, _: str) -> None:
        await self._member.send("You have already been verified!")

    @staticmethod
    def _generate_verification_code() -> str:
        alphabet = string.ascii_letters + string.digits
        return "".join(secrets.choice(alphabet) for i in range(8))

    @classmethod
    async def handle_message(
        cls, email_client: EmailClient, msg: discord.Message
    ) -> bool:
        """Handle a message from a user"""
        if msg.author.id not in cls._users:
            db: Database = Database()
            member = db.get_member(msg.author.id)
            if member and member.state in (UserState.GRADUATE, UserState.STUDENT):
                await msg.author.send(cls._already_verified_message())
                return False
            cls._users[msg.author.id] = cls(
                cast(discord.Member, msg.author), email_client
            )
        return await cls._users[msg.author.id].handle(msg.content)

    @classmethod
    async def add(cls, email_client: EmailClient, member: discord.Member) -> None:
        """Add a member to the verification process"""
        if member.id not in cls._users:
            cls._users[member.id] = cls(member, email_client)
        await cls._users[member.id].handle()

    @staticmethod
    def _already_verified_message() -> str:
        return "You are already verified! If you believe this is an error, please contact a STAIR member."  # pylint: disable=line-too-long
