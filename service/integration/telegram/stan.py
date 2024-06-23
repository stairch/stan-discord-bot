# -*- coding: utf-8 -*-
"""Telegram integration module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import os
import telegram

from pyaddict import JList

from db.datamodels.announcement import Announcement
from .helper import actual_markdown_to_markdownv2, base64_image_to_telegram


class Stan:
    """Telegram bot"""

    _ALLOWED_CHATS = (
        JList(os.getenv("TELEGRAM_CHATS", "").split(",")).iterator().ensureCast(int)
    )
    _TELEGRAM_BOT_TOKEN = os.getenv("TELEGRAM_BOT_TOKEN", "")

    def __init__(self) -> None:
        self._bot = telegram.Bot(Stan._TELEGRAM_BOT_TOKEN)
        self._chats: dict[int, telegram.ChatFullInfo] = {}

    async def start(self) -> None:
        """Start the bot"""
        async with self._bot:
            for chat_id in Stan._ALLOWED_CHATS:
                self._chats[chat_id] = await self._bot.get_chat(chat_id)

    @property
    def chats(self) -> dict[int, telegram.ChatFullInfo]:
        """Get all chats"""
        return self._chats

    async def send_announcement(
        self,
        announcement: Announcement,
        chat_id: int,
        image: str | None = None,
    ) -> bool:
        """Send an announcement to all chats"""
        if chat_id not in self._chats:
            return False
        markdown = f"""**{announcement.title}**

{announcement.message_de}

---

**{announcement.message_en}**"""

        async with self._bot:
            if image:
                await self._bot.send_photo(
                    chat_id,
                    base64_image_to_telegram(image),
                    caption=actual_markdown_to_markdownv2(markdown),
                    parse_mode="MarkdownV2",
                )
            else:
                await self._bot.send_message(
                    chat_id,
                    actual_markdown_to_markdownv2(markdown),
                    parse_mode="MarkdownV2",
                )
        return True
