# -*- coding: utf-8 -*-
"""Discord server wrapper"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import json
from dataclasses import dataclass
import re
import discord

STUDENT_ROLE = "Student"
GRADUATE_ROLE = "Graduate"


@dataclass
class ModuleDefinition:
    """A module with a name and a list of roles"""

    short: str
    role: str
    channel: str
    category: str
    colour: str


with open("./courses.json", "r+", encoding = "utf8") as f:
    COURSE_CONFIG = [ ModuleDefinition(**x)
                       for x in json.load(f)
                       if x.get("short") != "???" ]

class DiscordServer:
    """provides the student and graduate roles for a guild (server)"""

    def __init__(self, guild: discord.Guild) -> None:
        self._guild = guild
        self._roles = {role.name.lower(): role for role in guild.roles}
        self._student_role = self._roles.get(STUDENT_ROLE.lower())
        self._graduate_role = self._roles.get(GRADUATE_ROLE.lower())
        self._courses: list[ModuleDefinition] = COURSE_CONFIG

    def get_course_role(self, course: str) -> discord.Role:
        """Get the role for a course"""
        definition = next((x.role
                    for x in self._courses
                    if re.match(f"^\w+\.{x.short}.*\.\d+$", course)), None)
        assert definition is not None
        return self._roles[definition.lower()]

    async def create_course_roles(self) -> None:
        """Create the course roles"""
        has_categories = self._guild.categories
        has_roles = self._guild.roles
        for course in self._courses:
            category = next((x for x in has_categories if x.name == course.category), None)
            if not category:
                category = await self._guild.create_category(course.category)
                await self._guild.fetch_channels()
                has_categories = self._guild.categories
            channel = next((x for x in category.channels if x.name == course.channel), None)
            if not channel:
                await self._guild.create_text_channel(
                    course.channel, category=category,
                    overwrites = {
                        self._guild.default_role: discord.PermissionOverwrite(
                            view_channel = False,
                            send_messages = False),
                        self.get_student_role(): discord.PermissionOverwrite(
                            view_channel = True,
                            send_messages = False),
                    }
                )

            if not any(x.name == course.role for x in has_roles):
                await self._guild.create_role(
                    name=course.role,
                    mentionable=True,
                    color=discord.Color.from_str(course.colour),
                )
                self._roles = {role.name.lower(): role for role in await self._guild.fetch_roles()}
                has_roles = self._guild.roles

    def get_student_role(self) -> discord.Role:
        """Get the student role"""
        assert self._student_role is not None
        return self._student_role

    def get_graduate_role(self) -> discord.Role:
        """Get the graduate role"""
        assert self._graduate_role is not None
        return self._graduate_role

    def get_member(self, discord_id: int) -> discord.Member | None:
        """Get a member by their Discord ID, returning None if not found in guild"""
        return self._guild.get_member(discord_id)
