# -*- coding: utf-8 -*-
"""Singleton metaclass"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any


class Singleton(type):
    """Metaclass for Singletons"""

    _instances: dict[type, Any] = {}

    def __call__(cls, *args: Any, **kwds: Any) -> Any:
        if cls not in cls._instances:
            cls._instances[cls] = super(Singleton, cls).__call__(*args, **kwds)
        return cls._instances[cls]
