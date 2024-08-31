# -*- coding: utf-8 -*-
"""Discord server wrapper"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from enum import StrEnum
import re

import discord

from db.db import Database
from db.datamodels.announcement import AnnouncementType
from db.datamodels.degree_programme import DegreeProgramme


class RoleType(StrEnum):
    """Member roles"""

    STUDENT = "Student"
    GRADUATE = "Graduate"
    STAIR = "STAIR"

    def get(self, guild: discord.Guild) -> discord.Role:
        """Get the role for this type"""
        role = discord.utils.get(guild.roles, name=self.value)
        assert role is not None
        return role


class AnnouncementRoleType(StrEnum):
    """Announcement roles"""

    STAIR = "STAIR Notifications"
    NON_STAIR = "Non-STAIR Notifications"
    SERVER = "Server Notifications"
    TEST = "administrator"
    HUNGRY_STUDENT = "Hungry Student"

    def get(self, guild: discord.Guild) -> discord.Role:
        """Get the role for this type"""
        role = discord.utils.get(guild.roles, name=self.value)
        assert role is not None
        return role


class AnnouncementChannelType(StrEnum):
    """Announcement channels"""

    STAIR_CHANNEL = "stair-announcements"
    NON_STAIR_CHANNEL = "non-stair-announcements"
    SERVER_CHANNEL = "server-info"
    TEST_CHANNEL = "webhook-test"
    CANTEEN_MENU_CHANNEL = "canteen-menu"

    def get(self, guild: discord.Guild) -> discord.TextChannel | None:
        """Get the channel for this type"""
        return next(
            (
                channel
                for channel in guild.text_channels
                if channel.name.lower().endswith("︱" + self.value)
            ),
            None,
        )

    def get_role(self, guild: discord.Guild) -> discord.Role:
        """Get the role for this type"""
        role_type = {
            AnnouncementChannelType.STAIR_CHANNEL: AnnouncementRoleType.STAIR,
            AnnouncementChannelType.NON_STAIR_CHANNEL: AnnouncementRoleType.NON_STAIR,
            AnnouncementChannelType.SERVER_CHANNEL: AnnouncementRoleType.SERVER,
            AnnouncementChannelType.TEST_CHANNEL: AnnouncementRoleType.TEST,
            AnnouncementChannelType.CANTEEN_MENU_CHANNEL: AnnouncementRoleType.HUNGRY_STUDENT,
        }[self]
        return role_type.get(guild)

    @classmethod
    def from_announcement_type(
        cls, announcement_type: AnnouncementType
    ) -> AnnouncementChannelType:
        """Get the channel type for an announcement type"""
        return {
            AnnouncementType.STAIR: cls.STAIR_CHANNEL,
            AnnouncementType.NON_STAIR: cls.NON_STAIR_CHANNEL,
            AnnouncementType.SERVER_INFO: cls.SERVER_CHANNEL,
            AnnouncementType.TEST: cls.TEST_CHANNEL,
            AnnouncementType.CANTEEN_MENU: cls.CANTEEN_MENU_CHANNEL,
        }[announcement_type]


class DiscordServer:
    """provides the student and graduate roles for a guild (server)"""

    def __init__(self, guild: discord.Guild) -> None:
        self._guild = guild
        self._roles = {role.name.lower(): role for role in guild.roles}
        self._db: Database = Database()

    def get_announcement_role(self, role_type: AnnouncementRoleType) -> discord.Role:
        """Get the role for an announcement type"""
        return role_type.get(self._guild)

    @property
    def _courses(self) -> list[DegreeProgramme]:
        """Get the courses"""
        return self._db.get_degree_programmes()

    def get_course_role(self, course: str) -> discord.Role:
        """Get the role for a course"""
        definition = next(
            (
                x.role
                for x in self._courses
                if re.match(
                    r"^\w+\.{}.*\.\d+$".format(x.id),  # pylint: disable=consider-using-f-string
                    course,
                )
            ),
            None,
        )
        assert definition is not None
        return self._roles[definition.lower()]

    def get_course_roles_except(self, course: str | None = None) -> list[discord.Role]:
        """
        Get all roles that this user should not have
        """
        roles = [
            self._roles.get(x.role.lower(), None)
            for x in self._courses
            if course is None
            or not re.match(
                r"^\w+\.{}.*\.\d+$".format(x.id),  # pylint: disable=consider-using-f-string
                course,
            )
        ]
        return [x for x in roles if x is not None]

    async def create_course_roles(self) -> None:
        """Create the course roles"""
        has_categories = self._guild.categories
        has_roles = self._guild.roles
        for course in self._courses:
            category = next(
                (x for x in has_categories if x.name == course.category), None
            )
            if not category:
                category = await self._guild.create_category(course.category)
                await self._guild.fetch_channels()
                has_categories = self._guild.categories
            channel = next(
                (x for x in category.channels if x.name == course.channel), None
            )
            if not channel:
                await self._guild.create_text_channel(
                    course.channel,
                    category=category,
                    overwrites={
                        self._guild.default_role: discord.PermissionOverwrite(
                            view_channel=False, send_messages=False
                        ),
                        self.get_member_role(
                            RoleType.STUDENT
                        ): discord.PermissionOverwrite(
                            view_channel=True, send_messages=False
                        ),
                        self.get_member_role(
                            RoleType.STAIR
                        ): discord.PermissionOverwrite(
                            view_channel=True, send_messages=True
                        ),
                    },
                )

            if not any(x.name == course.role for x in has_roles):
                await self._guild.create_role(
                    name=course.role,
                    mentionable=True,
                    color=discord.Color.from_str(course.colour),
                )
                self._roles = {
                    role.name.lower(): role for role in await self._guild.fetch_roles()
                }
                has_roles = self._guild.roles

    def get_member_role(self, role_type: RoleType) -> discord.Role:
        """Get the role for a member type"""
        return role_type.get(self._guild)

    def get_member(self, discord_id: int) -> discord.Member | None:
        """Get a member by their Discord ID, returning None if not found in guild"""
        return self._guild.get_member(discord_id)

    @property
    def id(self) -> int:
        """Get the ID of the guild"""
        return self._guild.id

    @property
    def name(self) -> str:
        """Get the name of the guild"""
        return self._guild.name

    def serialise(self) -> dict[str, str]:
        """Serialise the server"""
        return {
            "id": str(self._guild.id),
            "name": self.name,
            "picture": self._guild.icon.url if self._guild.icon else "",
        }

    @property
    def guild(self) -> discord.Guild:
        """Get the guild"""
        return self._guild
