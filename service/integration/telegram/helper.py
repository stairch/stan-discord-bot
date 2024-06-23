# -*- coding: utf-8 -*-
"""Telegram helper module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import re
import base64


def actual_markdown_to_markdownv2(text: str) -> str:
    """Convert actual markdown to telegram-required markdownv2"""
    text = re.sub(r"(?<!\*)\*(?!\*)", "_", text)
    text = text.replace("**", "*")
    text = re.sub(r"([!\-.()])", r"\\\1", text)
    return text


def base64_image_to_telegram(base64_string: str) -> bytes:
    """Convert a base64 image to a telegram image"""
    return base64.b64decode(base64_string)
