# -*- coding: utf-8 -*-
"""State machine for verifying students."""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from enum import Enum
from typing import Awaitable, Callable, cast
import re
import asyncio
import logging
import string
import secrets
from datetime import datetime

import discord

from db.db import Database
from db.datamodels.verified_user import UserState
from integration.email.client import EmailClient


HACKSTAIR_UPCOMING = True


class VerificationState(Enum):
    """the state of the verification process"""

    PENDING = 0
    WAITING_FOR_EMAIL = 1
    WAITING_FOR_CODE = 2
    VERIFIED = 3
    EXPIRED = 4


class VerifyingStudent:  # pylint: disable=too-many-instance-attributes
    """State machine for verifying students"""

    _users: dict[int, VerifyingStudent] = {}
    OnHackStairT = Callable[[discord.Member], Awaitable[None]]

    def __init__(
        self,
        member: discord.Member,
        email_client: EmailClient,
        on_hackstair: OnHackStairT | None = None,
    ) -> None:
        self._member: discord.Member = member
        self._email_client: EmailClient = email_client
        self._state = VerificationState.PENDING
        self._verification_code = self._generate_verification_code()
        self._email: str | None = None
        self._on_hackstair = on_hackstair
        self._db: Database = Database()
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

    async def _hackstair_callback(self, interaction: discord.Interaction) -> None:
        await interaction.response.send_message(
            """Alright! You have now access to the HackSTAIR community!
Check out https://hack.stair.ch for more information.
"""
        )
        if self._on_hackstair:
            await self._on_hackstair(self._member)

    async def _student_callback(self, interaction: discord.Interaction) -> None:
        self._state = VerificationState.WAITING_FOR_EMAIL
        await interaction.response.send_message(
            "Great! Please provide your HSLU student email address (ending in `@stud.hslu.ch`)."
        )

    async def _handle_pending(self, msg: str) -> None:
        self._state = VerificationState.WAITING_FOR_EMAIL

        if msg:
            await self._handle_waiting_for_email(msg)
            return

        message = "Welcome to the server!"

        view: discord.ui.View = discord.ui.View()
        student_button: discord.ui.Button = discord.ui.Button(
            label="I'm a HSLU.I student",
            style=discord.ButtonStyle.primary,
            emoji="🎓",
        )
        student_button.callback = self._student_callback  # type: ignore
        view.add_item(student_button)
        if HACKSTAIR_UPCOMING:
            hackstair_button: discord.ui.Button = discord.ui.Button(
                label="Join the HackSTAIR community",
                style=discord.ButtonStyle.secondary,
                emoji="🤖",
            )
            hackstair_button.callback = self._hackstair_callback  # type: ignore
            view.add_item(hackstair_button)
        await self._member.send(message, view=view)

    @staticmethod
    def _generic_validation_error_message() -> str:
        base = """
⛔ Your email address is already taken, invalid or you are not a student at HSLU-I. ⛔

You must provide a valid email address ending in `@stud.hslu.ch`.
Please try again or contact a STAIR member.
        """
        # current month is july - september or december - february
        if (
            7 <= datetime.now().month <= 9
            or 12 <= datetime.now().month
            or datetime.now().month <= 2
        ):
            base += """
If you are a new student, please try again after the first week of the semester.
            """
        return base

    async def _handle_waiting_for_email(self, msg: str) -> None:
        email = re.search(r"[a-zA-Z0-9_.+-]+@stud.hslu.ch", msg)

        if not email:
            await self._member.send(self._generic_validation_error_message())
            return
        student = self._db.student_by_email(email.group().lower())

        if not student:
            await self._member.send(self._generic_validation_error_message())
            return

        email_norm = email.group().lower()

        if self._db.get_member(discord_id=self._member.id, email=email_norm):
            await self._member.send(self._generic_validation_error_message())
            return

        self._email = email_norm
        await self._send_verification_code(
            f"Nice to meet you, {student.first_name.split(' ')[0]}! I sent you an email with a verification code. Please provide it here."  # pylint: disable=line-too-long
        )

    async def _send_verification_code(self, msg: str) -> None:
        verification_code_splice = (
            self._verification_code[:4] + " " + self._verification_code[4:]
        )
        await self._email_client.send_email(
            "STAIR Discord Verification Code",
            f"""Hi there!
<br />
Your verification code is:
<h1 style="color: #04956c">{verification_code_splice}</h1>

<span style="font-size: 0.75rem; color: #22222A">Pro tip: Send me this code on Discord to verify your account and get full access to the server!</span>
""",
            self.email,
        )
        await self._member.send(msg)
        self._state = VerificationState.WAITING_FOR_CODE
        asyncio.create_task(self._expire())

    async def _handle_waiting_for_code(self, msg: str) -> bool:
        if msg.replace(" ", "") != self._verification_code:
            await self._member.send("Invalid verification code. Please try again.")
            return False

        self._state = VerificationState.VERIFIED
        try:
            self._db.verify_member(self._member.id, self.email)
        except Exception as e:  # pylint: disable=broad-except
            self._logger.exception(e)
            await self._member.send(
                "Oops, that didn't work. Please try again or contact a STAIR member."
            )
            return False

        await self._member.send("Awesome! You have been verified! ✅")
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
        alphabet = string.ascii_uppercase + string.digits
        return "".join(secrets.choice(alphabet) for i in range(8))

    @classmethod
    async def handle_message(
        cls,
        email_client: EmailClient,
        msg: discord.Message,
        on_hackstair: OnHackStairT | None = None,
    ) -> bool:
        """Handle a message from a user"""
        if msg.author.id not in cls._users:
            db: Database = Database()
            member = db.get_member(msg.author.id)
            if member and member.state in (UserState.GRADUATE, UserState.STUDENT):
                await msg.author.send(cls._already_verified_message())
                return False
            cls._users[msg.author.id] = cls(
                cast(discord.Member, msg.author),
                email_client,
                on_hackstair,
            )
        return await cls._users[msg.author.id].handle(msg.content)

    @classmethod
    async def add(
        cls,
        email_client: EmailClient,
        member: discord.Member,
        on_hackstair: OnHackStairT | None = None,
    ) -> None:
        """Add a member to the verification process"""
        if member.id not in cls._users:
            cls._users[member.id] = cls(member, email_client, on_hackstair)
        await cls._users[member.id].handle()

    @staticmethod
    def _already_verified_message() -> str:
        return "You are already verified! If you believe this is an error, please contact a STAIR member."  # pylint: disable=line-too-long
