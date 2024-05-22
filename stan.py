# -*- coding: utf-8 -*-
"""The main bot class for Stan."""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging
import os

import discord

from email_client import EmailClient
from verifying_student import VerifyingStudent
from guild import DiscordServer
from db.db import Database
from db.datamodels.verified_user import VerifiedUser


class Stan(discord.Client):
    """The main bot class for Stan."""

    _ALLOWED_GUILDS = [1240326708284887130]
    _DISCORD_APP_ID = os.getenv("DISCORD_APP_ID", "")

    def __init__(self) -> None:
        intents = discord.Intents.default()
        intents.message_content = True
        intents.guilds = True
        intents.members = True
        super().__init__(intents=intents)
        self._email_client = EmailClient()
        self._logger = logging.getLogger("Stan")
        self._servers: list[DiscordServer] = []
        self._db: Database = Database()

    async def on_ready(self):
        """Bot is ready"""
        self._logger.info("We have logged in as %s", self.user)
        for guild in self.guilds:
            if guild.id not in Stan._ALLOWED_GUILDS:
                self._logger.warning("unexpected guild %s", guild)
                continue
            self._servers.append(DiscordServer(guild))

    async def on_message(self, message: discord.Message):
        """Message handler"""
        if message.author == self.user:
            return

        if not any(x.id in Stan._ALLOWED_GUILDS for x in message.author.mutual_guilds):
            self._logger.debug("unexpected guild %s", message.guild)
            await message.author.send(
                "I'm not allowed to interact with you. Please join our discord server to get started. If you believe this is an error, please contact a STAIR member."  # pylint: disable=line-too-long
            )
            return

        if not message.guild:
            make_student = await VerifyingStudent.handle_message(
                self._email_client, message
            )
            if make_student:
                member = self._db.get_member(message.author.id)
                if not member:
                    return
                await self.make_student(member)
            return

        if message.guild.id not in Stan._ALLOWED_GUILDS:
            self._logger.debug("unexpected guild %s", message.guild)
            return

    async def make_graduate(self, user: VerifiedUser) -> None:
        """applies the graduate role to a user on all supported servers"""
        for server in self._servers:
            member = server.get_member(user.discord_id)
            if member is None:
                continue
            await member.remove_roles(
                server.get_student_role(), *server.get_course_roles_except()
            )
            await member.add_roles(server.get_graduate_role())

    async def make_student(self, user: VerifiedUser) -> None:
        """applies the student role to a user on all supported servers"""
        for server in self._servers:
            member = server.get_member(user.discord_id)
            student = self._db.student_by_email(user.email)
            if member is None or student is None:
                continue
            await member.remove_roles(
                server.get_graduate_role(),
                *server.get_course_roles_except(student.course_id),
            )
            await member.add_roles(
                server.get_student_role(), server.get_course_role(student.course_id)
            )

    async def _move_module_channels(self, message: discord.Message):
        target: discord.CategoryChannel | None = None
        old: list[discord.CategoryChannel] = []
        assert message.guild

        for category in message.guild.categories:
            if category.name == "MODULE CHANNELS":
                target = category
                continue
            if category.name.startswith("MODULE CHANNELS "):
                old.append(category)

        if target is None:
            target = await message.guild.create_category("MODULE CHANNELS")

        for category in old:
            for channel in category.channels:
                self._logger.debug("edit channel %s to %s", channel.name, target.name)
                await channel.edit(category=target, sync_permissions=True)  # type: ignore
            self._logger.debug("delete category %s", category.name)
            await category.delete()

    async def on_member_join(self, member: discord.member.Member):
        """Member joined the server"""
        self._logger.debug("%s has joined the server", member)
        await VerifyingStudent.add(self._email_client, member)

    async def start(self, token: str = "", *, reconnect: bool = True):
        """Start the bot"""
        await super().start(token or self._DISCORD_APP_ID, reconnect=reconnect)
