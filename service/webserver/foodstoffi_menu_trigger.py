# -*- coding: utf-8 -*-
"""Announcement handler module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"


from aiohttp import web

from .base_handler import BaseHandler
from .msal_auth.auth import authenticated


class FoodstoffMenuTrigger(BaseHandler):
    """Handler for manual foodstoffi menu triggers"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_post("/api/subscriptions/foodstoffi/trigger", self._trigger)

    @authenticated
    async def _trigger(self, _: web.Request) -> web.Response:
        """Trigger a manual foodstoffi menu update"""
        self._logger.info("Manual foodstoffi menu trigger")
        await self._integration.trigger_foodstoffi_menu()
        return web.json_response({"message": "Foodstoffi menu update triggered"})
