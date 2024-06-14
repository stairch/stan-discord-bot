# -*- coding: utf-8 -*-
"""Utility functions for discord"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import io
import base64

import discord


def base64_image_to_discord(
    base64_string: str | None, file_name: str = "image.png"
) -> discord.File | None:
    """Convert a base64 image to a discord file"""
    if not base64_string:
        return None
    return discord.File(io.BytesIO(base64.b64decode(base64_string)), filename=file_name)
