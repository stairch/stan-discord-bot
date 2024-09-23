# -*- coding: utf-8 -*-
"""Personas that the bot can use"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import logging

import discord

from common.configurable_enum import ConfigurableEnum


class Persona(ConfigurableEnum):
    """Persona"""

    _config_path = "/personas/definition.json"

    @property
    def avatar_location(self) -> str:
        """Avatar location"""
        return self._global_data.ensure("basePath", str) + self._data.ensure(
            "avatar", str
        )

    @property
    def avatar(self) -> bytes:
        """Avatar"""
        with open(self.avatar_location, "rb") as file:
            return file.read()

    @staticmethod
    def default() -> Persona:
        """Default persona"""
        return Persona("Stan", {}, {})


class PersonaSender:
    """Module Channel Sync"""

    def __init__(self, channel: discord.TextChannel, persona: Persona) -> None:
        self._channel = channel
        self._persona = persona
        self._logger = logging.getLogger(__name__)

    async def send(
        self,
        message: str = discord.utils.MISSING,
        embeds: list[discord.Embed] = discord.utils.MISSING,
        file: discord.File | None = discord.utils.MISSING,
        publish: bool = True,
    ) -> discord.Message:
        """Send a message with a persona"""
        file = file or discord.utils.MISSING

        if self._persona == Persona.default():
            msg = await self._channel.send(message, embeds=embeds, file=file)
        else:
            webhooks = await self._channel.guild.webhooks()
            webhook = next(
                (wh for wh in webhooks if wh.name == self._persona.name), None
            )
            if webhook:
                await webhook.edit(
                    name=self._persona.name,
                    avatar=self._persona.avatar,
                    channel=self._channel,
                )
            else:
                webhook = await self._channel.create_webhook(
                    name=self._persona.name, avatar=self._persona.avatar
                )
            msg = await webhook.send(
                content=message,
                embeds=embeds,
                file=file,
                wait=True,
            )
        if publish:
            try:
                await msg.publish()
            except discord.errors.Forbidden as e:
                self._logger.warning("Failed to publish message: %s", e)
                return msg
        return msg
