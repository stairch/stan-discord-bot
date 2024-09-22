# -*- coding: utf-8 -*-
"""MSAL Auth"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import os
from typing import Any
from enum import StrEnum
from functools import wraps

from aiohttp import web
import aiohttp
from aiohttp_session import get_session, new_session, Session
from msal import PublicClientApplication, SerializableTokenCache  # type: ignore


CALLBACK = "/api/auth/callback"
COOKIE_NAME = "msal_session"

AD_APP_ID = os.getenv("AD_APP_ID")
AD_TENANT_ID = os.getenv("AD_TENANT_ID")
EMAIL_ADDRESS = os.getenv("EMAIL_ADDRESS", "")


class SessionKeys(StrEnum):
    """Session keys."""

    FLOW_CACHE = "flow_cache"
    SESSION_REDIRECT = "session_redirect"
    MS_REDIRECT = "ms_redirect"
    MAIL = "mail"
    USER_DATA = "user_data"


def authenticated(
    function: Any,
) -> Any:
    """caches the response for a specified time"""

    @wraps(function)
    async def _wrapper(*args: Any, **kwargs: Any) -> web.Response:
        # args can be (self, request) or (request)
        request: web.Request = args[1] if len(args) > 1 else args[0]
        session = await MsalSession.get(request)
        if not session.user_data:
            return web.HTTPForbidden(text="Not authenticated")
        return await function(*args, **kwargs)

    return _wrapper


async def get_username(request: web.Request) -> str:
    """Get username."""
    session = await MsalSession.get(request)
    return session.user_data.get("displayName", "")


class MsalSession:
    """MSAL Session Helper."""

    _token_cache = SerializableTokenCache()

    def __init__(self, session: Session) -> None:
        self._session = session
        self._app = None

    @property
    def flow_cache(self) -> dict[str, Any]:
        """Get flow_cache."""
        return self._session.get(SessionKeys.FLOW_CACHE) or {}

    @flow_cache.setter
    def flow_cache(self, flow_cache: dict[str, Any]) -> None:
        """Set flow_cache."""
        self._session[SessionKeys.FLOW_CACHE] = flow_cache

    @property
    def session_redirect(self) -> str:
        """The redirect for the user to return to after login."""
        return self._session.get(SessionKeys.SESSION_REDIRECT) or "/"

    @session_redirect.setter
    def session_redirect(self, session_redirect: str) -> None:
        self._session[SessionKeys.SESSION_REDIRECT] = session_redirect

    @property
    def ms_redirect(self) -> str:
        """The redirect that we pass to Microsoft (our callback handler)"""
        return self._session.get(SessionKeys.MS_REDIRECT) or ""

    @ms_redirect.setter
    def ms_redirect(self, ms_redirect: str) -> None:
        self._session[SessionKeys.MS_REDIRECT] = ms_redirect

    @property
    def mail(self) -> str:
        """Get mail."""
        return self._session.get(SessionKeys.MAIL) or ""

    @mail.setter
    def mail(self, mail: str) -> None:
        """Set mail."""
        self._session[SessionKeys.MAIL] = mail

    @property
    def user_data(self) -> dict[str, Any]:
        """Get user_data."""
        return self._session.get(SessionKeys.USER_DATA) or {}

    @user_data.setter
    def user_data(self, user_data: dict[str, Any]) -> None:
        """Set user_data."""
        self._session[SessionKeys.USER_DATA] = user_data

    def initiate_auth_code_flow(self) -> str:
        """Initiate auth code flow, return auth_uri."""
        self.flow_cache = self.app.initiate_auth_code_flow(
            scopes=[],
            redirect_uri=self.ms_redirect,
            domain_hint=EMAIL_ADDRESS.split("@").pop(),
            response_mode="form_post",
        )
        if "error" in self.flow_cache:
            raise web.HTTPForbidden(text=self.flow_cache["error"])
        return self.flow_cache["auth_uri"]

    async def acquire_token_by_auth_code_flow(
        self, auth_response: dict[str, Any]
    ) -> None:
        """Acquire token by auth code flow."""
        data = self.app.acquire_token_by_auth_code_flow(self.flow_cache, auth_response)
        if "error" in data:
            raise web.HTTPForbidden(text=data["error"])
        token = data["access_token"]
        async with aiohttp.ClientSession() as client_session:
            async with client_session.get(
                "https://graph.microsoft.com/v1.0/me",
                headers={"Authorization": f"Bearer {token}"},
            ) as resp:
                self.user_data = await resp.json()

    @property
    def app(self) -> PublicClientApplication:
        """Get app."""
        if not self._app:
            self._app = PublicClientApplication(
                client_id=AD_APP_ID,
                authority=f"https://login.microsoftonline.com/{AD_TENANT_ID}",
                token_cache=MsalSession._token_cache,
            )
        return self._app

    @classmethod
    async def create(cls, request: web.Request) -> "MsalSession":
        """Create a new session."""
        session = await new_session(request)
        instance = cls(session)
        referer = request.headers.get("Referer", "")
        # server name
        if referer:
            scheme = request.scheme
            server_name = referer.split("/")[2]
            if "localhost" not in server_name:
                scheme = "https"
            instance.ms_redirect = f"{scheme}://{server_name}{CALLBACK}"
        else:
            instance.ms_redirect = f"{request.scheme}://localhost{CALLBACK}"
        return instance

    @classmethod
    async def get(cls, request: web.Request) -> "MsalSession":
        """Get an existing session."""
        session = await get_session(request)
        return cls(session)
