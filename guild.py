# -*- coding: utf-8 -*-
"""Discord server wrapper"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio
import discord

STUDENT_ROLE = "Student"
GRADUATE_ROLE = "Graduate"


class DiscordServer:
    """provides the student and graduate roles for a guild (server)"""

    def __init__(self, guild: discord.Guild) -> None:
        self._guild = guild
        self._roles = {role.name.lower(): role for role in guild.roles}
        self._student_role = self._roles.get(STUDENT_ROLE.lower())
        self._graduate_role = self._roles.get(GRADUATE_ROLE.lower())
        asyncio.create_task(guild.fetch_roles())

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
