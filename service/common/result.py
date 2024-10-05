# -*- coding: utf-8 -*-
"""Optional class"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Callable

from aiohttp import web


class Result[T]:
    """Can wrap a value or an error"""

    def __init__(
        self, *, value: T, error: str | None = None, status: int | None = None
    ):
        self._value = value
        self._error = error
        self._status = status

    def is_ok(self) -> bool:
        """Check if the result is ok"""
        return self._error is None

    def is_err(self) -> bool:
        """Check if the result is an error"""
        return self._error is not None

    def unwrap(self) -> T:
        """Unwrap the value, if it is an error, raise an exception"""
        if self._error is not None:
            raise ValueError("Result is not ok")
        return self._value

    def unwrap_err(self) -> str:
        """Unwrap the error, if it is a value, raise an exception"""
        if self._error is None:
            raise ValueError("Result is not an error")
        return self._error

    def map[U](self, f: Callable[[T], U]) -> Result[U | None]:
        """Map the value to a new value"""
        if self._error is not None:
            return Result(value=None, error=self._error)
        return Result(value=f(self._value))

    def map_err(self, f: Callable[[str], str]) -> Result[T | None]:
        """Map the error to a new error"""
        if self._error is None:
            return Result(value=self._value)
        return Result(value=None, error=f(self._error))

    def __str__(self) -> str:
        if self._error is not None:
            return f"Error: {self._error}"
        return f"Ok: {self._value}"

    def as_web_response(self) -> web.Response:
        """Return the result as a web response"""
        if self._error is not None:
            return web.Response(text=self._error, status=self._status or 400)
        return web.json_response({"value": self._value}, status=self._status or 200)

    @classmethod
    def ok(cls, value: T, status: int = 200) -> Result[T]:
        """Create a new result with a value"""
        return Result(value=value, status=status)

    @classmethod
    def err(cls, error: str, status: int = 400) -> Result[None]:
        """Create a new result with an error"""
        return Result(value=None, error=error, status=status)
