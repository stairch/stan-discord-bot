# -*- coding: utf-8 -*-
"""The main bot class for Stan."""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging
import os

import discord
from pyaddict import JList

from db.db import Database
from db.datamodels.verified_user import VerifiedUser
from integration.email.client import EmailClient
from .verifying_student import VerifyingStudent
from .server import DiscordServer, RoleType


class Stan(discord.Client):
    """The main bot class for Stan."""

    _ALLOWED_GUILDS = (
        JList(os.getenv("DISCORD_SERVERS", "").split(",")).iterator().ensureCast(int)
    )
    _DISCORD_APP_ID = os.getenv("DISCORD_APP_ID", "")

    def __init__(self, email_client: EmailClient) -> None:
        intents = discord.Intents.default()
        intents.message_content = True
        intents.guilds = True
        intents.members = True
        super().__init__(intents=intents)
        self._email_client = email_client
        self._logger = logging.getLogger("Stan")
        self._servers: list[DiscordServer] = []
        self._db: Database = Database()

    async def start(self, token: str = "", *, reconnect: bool = True):
        """Start the bot"""
        await super().start(token or self._DISCORD_APP_ID, reconnect=reconnect)

    @property
    def servers(self) -> dict[int, DiscordServer]:
        """Get all servers"""
        return {x.id: x for x in self._servers}

    async def on_ready(self):
        """Bot is ready"""
        self._logger.info("We have logged in as %s", self.user)
        for guild in self.guilds:
            if guild.id not in Stan._ALLOWED_GUILDS:
                self._logger.warning(
                    "ignoring unknown guild %s. not specified in DISCORD_SERVERS", guild
                )
                continue
            self._servers.append(DiscordServer(guild))

    async def on_message(self, message: discord.Message):
        """Message handler"""
        if message.author == self.user:
            return

        if not message.guild and not any(
            x.id in Stan._ALLOWED_GUILDS for x in message.author.mutual_guilds
        ):
            await message.author.send(
                "I'm not allowed to interact with you. Please join our discord server to get started. If you believe this is an error, please contact a STAIR member."  # pylint: disable=line-too-long
            )
            return

        if not message.guild:
            make_student = await VerifyingStudent.handle_message(
                self._email_client, message, self.make_hackstair
            )
            if make_student:
                member = self._db.get_member(message.author.id)
                if not member:
                    return
                await self.make_student(member)
            return

        if message.guild.id not in Stan._ALLOWED_GUILDS:
            self._logger.debug(
                "ignoring unknown guild %s. not specified in DISCORD_SERVERS",
                message.guild,
            )
            return

    async def make_graduate(self, user: VerifiedUser) -> None:
        """applies the graduate role to a user on all supported servers"""
        for server in self._servers:
            member = server.get_member(user.discord_id)
            if member is None:
                continue
            await member.remove_roles(
                server.get_member_role(RoleType.STUDENT),
                *server.get_course_roles_except(),
            )
            await member.add_roles(
                server.get_member_role(RoleType.GRADUATE),
            )

    async def make_student(self, user: VerifiedUser) -> None:
        """applies the student role to a user on all supported servers"""
        for server in self._servers:
            member = server.get_member(user.discord_id)
            student = self._db.student_by_email(user.email)
            if member is None or student is None:
                continue
            await member.remove_roles(
                server.get_member_role(RoleType.GRADUATE),
                *server.get_course_roles_except(student.course_id),
            )
            to_add = [
                server.get_member_role(RoleType.STUDENT),
            ]
            if course_role := server.get_course_role(student.course_id):
                to_add.append(course_role)

            await member.add_roles(*to_add)

    async def make_hackstair(self, user: discord.Member) -> None:
        """applies the hackstair role to a user on all supported servers"""
        for server in self._servers:
            member = server.get_member(user.id)
            if member is None:
                continue
            await member.add_roles(
                server.get_member_role(RoleType.HACKSTAIR),
            )

    async def on_member_join(self, member: discord.Member):
        """Member joined the server"""
        self._logger.debug("%s has joined the server", member)
        await VerifyingStudent.add(self._email_client, member, self.make_hackstair)
