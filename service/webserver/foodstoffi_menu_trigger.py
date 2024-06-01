# -*- coding: utf-8 -*-
"""Announcement handler module"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"


from aiohttp import web

from .base_handler import BaseHandler


class FoodstoffMenuTrigger(BaseHandler):
    """Handler for manual foodstoffi menu triggers"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_post("/api/subscriptions/foodstoffi/trigger", self._trigger)

    async def _trigger(self, _: web.Request) -> web.Response:
        """Trigger a manual foodstoffi menu update"""
        self._logger.info("Manual foodstoffi menu trigger")
        await self._stan.send_foodstoffi_menu_update()
        return web.json_response({"message": "Foodstoffi menu update triggered"})
