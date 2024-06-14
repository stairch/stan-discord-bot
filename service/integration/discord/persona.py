# -*- coding: utf-8 -*-
"""Personas that the bot can use"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"


from dataclasses import dataclass
from enum import Enum
from pathlib import Path
import logging

import discord

ASSET_LOCATION = Path(__file__).parent / "avatars"


@dataclass
class _Persona:
    """Persona"""

    name: str
    avatar_location: str

    @property
    def avatar(self) -> bytes:
        """Avatar"""
        with open(ASSET_LOCATION / self.avatar_location, "rb") as file:
            return file.read()


class Personas(Enum):
    """Pre-defined personas"""

    STAN = _Persona("Stan", "default-hat.png")
    CHEF = _Persona("Chef Stan-dwich", "grill-hat.png")
    GRADUATE = _Persona("Gradua-Stan", "graduation-hat.png")
    PIRATE = _Persona("Captain Stan-tastic", "halloween-pirate-hat.png")
    WITCH = _Persona("Stan-dalf the Green", "halloween-witch-hat.png")
    PARTY = _Persona("Party Stan-imal", "party-hat.png")
    SANTA = _Persona("Santa Stan", "winter-hat.png")

    @classmethod
    def from_name(cls, name: str | None) -> Personas:
        """Get a persona by name"""
        return next((x for x in cls if x.value.name == name), cls.STAN)


class PersonaSender:
    """Module Channel Sync"""

    def __init__(self, channel: discord.TextChannel, persona: Personas) -> None:
        self._channel = channel
        self._persona = persona.value
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

        if self._persona == Personas.STAN.value:
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
                raise
        return msg
