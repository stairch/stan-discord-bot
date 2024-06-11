# -*- coding: utf-8 -*-
"""Integration manager module"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import asyncio

from .discord.stan import Stan
from .discord.module_channels import ModuleChannelSync
from .email.client import EmailClient
from .foodstoffi.menu import SendFoodstoffiMenuTask


class IntegrationManager:
    """Manager for the integration services"""

    def __init__(self) -> None:
        self._email_client = EmailClient()
        self._stan = Stan(self._email_client)
        self._send_foodstoffi_menu_task = SendFoodstoffiMenuTask(self._stan)
        self._module_channel_sync = ModuleChannelSync(self._stan)

    @property
    def stan(self) -> Stan:
        """Get the discord bot Stan"""
        return self._stan

    async def trigger_foodstoffi_menu(self) -> None:
        """Trigger a manual foodstoffi menu update"""
        await self._send_foodstoffi_menu_task.trigger()

    async def trigger_module_channel_sync(self, plain: str) -> None:
        """Trigger a manual module channel sync"""
        await self._module_channel_sync.sync(plain)

    async def start(self) -> None:
        """Start the integration services"""
        asyncio.create_task(self._stan.start())
        await self._send_foodstoffi_menu_task.start()
