# -*- coding: utf-8 -*-
"""MSAL Auth"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"


from urllib.parse import urljoin
from aiohttp import web
from aiohttp_session import get_session

from webserver.base_handler import BaseHandler
from .auth import CALLBACK, SessionKeys, MsalSession, authenticated


class MsalAuth(BaseHandler):
    """Microsoft Authentication."""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_get("/api/auth/signin", self._signin)
        app.router.add_get("/api/auth/signout", self._signout)
        app.router.add_get("/api/auth/me", self._user)
        app.router.add_post(CALLBACK, self._callback)

    async def _signin(self, request: web.Request) -> web.Response:
        """Redirect to MS login page."""
        session = await MsalSession.create(request)

        _to = request.match_info.get("to", "")
        ref = request.headers.getone("Referer", "")
        if ref:
            _to = urljoin(ref, _to)

        session.session_redirect = _to

        return web.HTTPFound(session.initiate_auth_code_flow())

    async def _callback(self, request: web.Request) -> web.Response:
        """Complete the auth code flow."""
        session = await MsalSession.get(request)

        # build a plain dict from the aiohttp server request's url parameters
        # pre-0.1.18. Now we have response_mode="form_post"
        # auth_response = dict(request.rel_url.query.items())
        auth_response = dict(await request.post())

        # Ensure all expected variables were returned...
        if not all(auth_response.get(k) for k in ["code", "session_state", "state"]):
            return web.HTTPPreconditionRequired(text="Missing auth_response")

        if not session.flow_cache:
            return web.HTTPPreconditionRequired(text="Session invalid")

        await session.acquire_token_by_auth_code_flow(auth_response)
        return web.HTTPFound(session.session_redirect)

    @authenticated
    async def _signout(self, request: web.Request) -> web.Response:
        """Redirect to MS graph login page."""
        session = await get_session(request)
        session.clear()

        # post_logout_redirect_uri
        _to = request.match_info.get("to", "")
        ref = request.headers.getone("Referer", "")
        if ref:
            _to = urljoin(ref, _to)

        return web.HTTPFound(
            "https://login.microsoftonline.com/common/oauth2/logout?"
            f"post_logout_redirect_uri={_to}"
        )

    @authenticated
    async def _user(self, request: web.Request) -> web.Response:
        """Get user data."""
        session = await get_session(request)
        return web.json_response(session[SessionKeys.USER_DATA])
