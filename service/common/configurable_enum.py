# -*- coding: utf-8 -*-
"""Configurable (dynamic) Enum"""
# mypy: disable-error-code=attr-defined

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any, overload
import os

from pyaddict import JDict
from pyaddict.schema import Object

_BASE_PATH = os.getenv("CONFIG_PATH", "")

_SCHEMA = Object(
    {
        "global": Object().withAdditionalProperties().optional(),
        "items": Object().withAdditionalProperties(),
    }
)


class MetaConfigurableEnum(type):
    """A class that represents a configurable enum."""

    _config_path: str | None = None
    _config: JDict = JDict()
    _items: dict[str, Any] = {}

    @overload
    def get[T](cls: type[T], key: str) -> T | None: ...

    @overload
    def get[T](cls: type[T], key: str, default: T) -> T: ...

    def get[T](cls: type[T], key: str | None, default: T | None = None) -> T | None:
        """Get an item from the configuration."""
        cls._load()  # pylint: disable=no-value-for-parameter
        if key is None:
            return default
        return next(
            (item for item in cls._items.values() if item.name.lower() == key.lower()),
            default,
        )

    def _load[T](cls: type[T]) -> None:
        if cls._config:
            return
        cls._config = JDict.fromFile(_BASE_PATH + cls._config_path)
        cls._verify_config(cls._config)  # pylint: disable=no-value-for-parameter
        global_config = cls._config.ensure("global", dict)
        cls._items = {
            k: cls(k, v, global_config)  # type: ignore
            for k, v in cls._config.ensure("items", dict).items()
        }

    def _verify_config(cls, config: dict[str, Any]) -> None:
        error = _SCHEMA.error(config)
        if error:
            raise ValueError(f"Invalid configuration: {error}")

    def __iter__(cls):
        cls._load()  # pylint: disable=no-value-for-parameter
        return iter(cls._items.values())

    def __getitem__(cls, key: str) -> Any:
        cls._load()  # pylint: disable=no-value-for-parameter
        return next(
            item for item in cls._items.values() if item.name.lower() == key.lower()
        )

    def __contains__(cls, key: str) -> bool:
        cls._load()  # pylint: disable=no-value-for-parameter
        return key.lower() in [x.lower() for x in cls._items]

    def __next__(cls) -> Any:
        cls._load()  # pylint: disable=no-value-for-parameter
        return next(cls.__iter__())  # pylint: disable=no-value-for-parameter

    def __len__(cls) -> int:
        cls._load()  # pylint: disable=no-value-for-parameter
        return len(cls._items)

    def serialise(cls) -> list[str]:
        """Serialise the configuration."""
        cls._load()  # pylint: disable=no-value-for-parameter
        return list(cls._items.keys())


class ConfigurableEnum(metaclass=MetaConfigurableEnum):
    """Base class for a configurable enums."""

    def __init__(
        self, name: str, data: dict[str, Any], global_data: dict[str, Any]
    ) -> None:
        self._name = name
        self._data = JDict(data)
        self._global_data = JDict(global_data)

    @property
    def name(self) -> str:
        """Get the name of the item."""
        return self._name

    def __str__(self) -> str:
        return self.name

    def __repr__(self) -> str:
        return f"<{self.__class__.__name__} {self.name}>"

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, ConfigurableEnum):
            return False
        return self.name == other.name
