# -*- coding: utf-8 -*-
"""Integration manager module"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio

from .discord.stan import Stan as DiscordStan
from .telegram.stan import Stan as TelegramStan
from .discord.module_channels import ModuleChannelSync
from .email.client import EmailClient
from .foodstoffi.menu import SendFoodstoffiMenuTask


class IntegrationManager:
    """Manager for the integration services"""

    def __init__(self) -> None:
        self._email_client = EmailClient()
        self._discord_stan = DiscordStan(self._email_client)
        self._telegram_stan = TelegramStan()
        self._send_foodstoffi_menu_task = SendFoodstoffiMenuTask(self._discord_stan)
        self._module_channel_sync = ModuleChannelSync(self._discord_stan)

    @property
    def discord(self) -> DiscordStan:
        """Get the discord bot Stan"""
        return self._discord_stan

    @property
    def telegram(self) -> TelegramStan:
        """Get the telegram bot Stan"""
        return self._telegram_stan

    async def trigger_foodstoffi_menu(self) -> None:
        """Trigger a manual foodstoffi menu update"""
        await self._send_foodstoffi_menu_task.trigger()

    async def trigger_module_channel_sync(self, plain: str) -> None:
        """Trigger a manual module channel sync"""
        await self._module_channel_sync.sync(plain)

    async def start(self) -> None:
        """Start the integration services"""
        asyncio.create_task(self._discord_stan.start())
        await self._telegram_stan.start()
        await self._send_foodstoffi_menu_task.start()
