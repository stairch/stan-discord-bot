# -*- coding: utf-8 -*-
"""Module Channels"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any
import csv
import re
import logging
from dataclasses import dataclass

from pyaddict import JDict
import discord

from .stan import Stan
from .server import DiscordServer, RoleType


@dataclass
class ModuleChannel:
    """Module Channel"""

    name: str
    credits: int
    shorthand: str
    course: str
    is_blockweek: bool

    @classmethod
    def from_dict(cls, obj: Any) -> "ModuleChannel | None":
        """Create a module channel from a dictionary"""
        data = JDict(obj)
        module_id = data.assertGet("Anlassnummer", str)
        if not module_id:
            return None
        shorthand = module_id.split(".")[1].removeprefix("BA_")
        shorthand = re.sub(r"_(K|E|MM|(F|H)\d{2})", "", shorthand)
        return cls(
            name=data.ensure("Anlassbezeichnung", str),
            credits=int(data.ensureCast("Credits", float)),
            shorthand=shorthand,
            course=data.ensure("Anlassleitung", str),
            is_blockweek=data.optionalGet("I.Blockwoche", str) == "Ja",
        )

    def category(self, i: int) -> str:
        """Get the category for the module channel"""
        tag = self.course
        if self.is_blockweek:
            tag += " Blockweeks "
        else:
            tag += " Modules "
        return tag + str(i)

    @property
    def description(self) -> str:
        """Get the description for the module channel"""
        return f"{self.name} ({self.credits} ECTS)"


class ModuleChannelSync:
    """Module Channel Sync"""

    def __init__(self, stan: Stan) -> None:
        self._stan = stan
        self._logger = logging.getLogger("module_channels")

    def _build_module_structure(self, plain: str) -> dict[str, ModuleChannel]:
        module_list = csv.DictReader(plain.splitlines())
        modules: dict[str, ModuleChannel] = {}
        for row in module_list:
            module = ModuleChannel.from_dict(row)
            if not module:
                continue
            modules[module.shorthand] = module
        return modules

    def _build_channel_structure(
        self, modules: dict[str, ModuleChannel]
    ) -> dict[str, list[dict[str, Any]]]:
        channels: dict[str, list[dict[str, Any]]] = {}
        for module in modules.values():
            for i in range(1, 50):
                tag = module.category(i)
                if tag not in channels:
                    channels[tag] = []
                elif len(channels[tag]) >= 50:
                    continue
                channels[tag].append(
                    {"name": module.shorthand, "description": module.description}
                )
                break
            else:
                self._logger.error("too many modules in %s", tag)
        return channels

    async def sync(self, plain: str) -> None:
        """Sync the module channels with the given CSV"""
        modules = self._build_module_structure(plain)
        channel_structure = self._build_channel_structure(modules)

        for server in self._stan.servers.values():
            await self._sync_server(server, channel_structure)

    async def _sync_server(
        self, server: DiscordServer, channel_structure: dict[str, list[dict[str, Any]]]
    ) -> None:
        discord_guild = server.guild
        old_categories: set[discord.CategoryChannel] = set()

        permissions: dict[
            discord.Role | discord.Member, discord.PermissionOverwrite
        ] = {
            discord_guild.default_role: discord.PermissionOverwrite(
                view_channel=False, send_messages=False
            ),
            server.get_member_role(RoleType.STUDENT): discord.PermissionOverwrite(
                view_channel=True, send_messages=False
            ),
        }

        for category, channels in channel_structure.items():
            existing_category = next(
                (x for x in discord_guild.categories if x.name == category), None
            )
            if existing_category:
                # then discard
                # a new category will be created
                # channels that continue to exist will be moved
                # old channels will stay in this category and we can delete them
                await existing_category.edit(name=f"OLD {category}")
                old_categories.add(existing_category)
            new_category = await discord_guild.create_category(
                category, overwrites=permissions
            )
            for channel in channels:
                existing_channel = next(
                    (
                        x
                        for x in discord_guild.channels
                        if x.name.lower() == channel["name"].lower()
                    ),
                    None,
                )
                self._logger.info(
                    "Syncing channel %s in category %s (existed=%s)",
                    channel["name"],
                    category,
                    bool(existing_channel),
                )

                # delete roles, if any
                existing_roles = [
                    x
                    for x in discord_guild.roles
                    if x.name.lower() == channel["name"].lower()
                ]
                for role in existing_roles:
                    await role.delete()

                if existing_channel:
                    # then update
                    if existing_channel.category:
                        # we will discard old categories
                        self._logger.info(
                            "Channel %s was in category %s",
                            existing_channel.name,
                            existing_channel.category.name,
                        )
                        old_categories.add(existing_channel.category)
                    assert isinstance(existing_channel, discord.TextChannel)
                    # update meta data & move to new category
                    await existing_channel.edit(
                        name=channel["name"],
                        topic=channel["description"],
                        category=new_category,
                        sync_permissions=True,
                    )
                else:
                    await new_category.create_text_channel(
                        name=channel["name"],
                        topic=channel["description"],
                    )
        await self._cleanup(discord_guild, old_categories)

    async def _cleanup(
        self, guild: discord.Guild, old_categories: set[discord.CategoryChannel]
    ) -> None:
        await guild.fetch_channels()
        for old_category in old_categories:
            self._logger.info("Deleting channel %s", old_category.name)
            for old_channel in old_category.channels:
                self._logger.info("Deleting channel %s", old_channel.name)
                await old_channel.delete()
            await old_category.delete()
