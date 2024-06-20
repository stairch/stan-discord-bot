# -*- coding: utf-8 -*-
"""DB import web handler"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from aiohttp import web

from db.datamodels.hslu_student import HsluStudent
from .base_handler import BaseHandler
from .msal_auth import authenticated


class DbImportHandler(BaseHandler):
    """Handler for importing students from CSV"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_post("/api/students", self._upload_students)
        app.router.add_get("/api/students", self._get_students)
        app.router.add_post("/api/modules", self._upload_modules)
        app.router.add_post("/api/degree-programmes", self._upload_degree_programmes)

    @authenticated
    async def _upload_students(self, request: web.Request) -> web.Response:
        """Upload students from a CSV file. These we'll write to the database."""
        plain = await request.text()
        new_graduates, new_students = self._db.update_students(
            HsluStudent.from_csv(plain)
        )
        for graduate in new_graduates:
            await self._integration.stan.make_graduate(graduate)
        for student in new_students:
            await self._integration.stan.make_student(student)
        return web.Response()

    @authenticated
    async def _get_students(self, _: web.Request) -> web.Response:
        """Get number of students in the database."""
        verified_users = self._db.all_verified()
        return web.json_response(
            {
                "enrolled": len(self._db.all_students()),
                "discord": {
                    "students": len([u for u in verified_users if u.is_student]),
                    "graduates": len([u for u in verified_users if u.is_graduate]),
                },
            }
        )

    @authenticated
    async def _upload_modules(self, request: web.Request) -> web.Response:
        """Upload modules from a CSV file."""
        plain = await request.text()
        await self._integration.trigger_module_channel_sync(plain)
        return web.Response()

    @authenticated
    async def _upload_degree_programmes(self, _: web.Request) -> web.Response:
        """Upload degree programmes from a CSV file. These we'll write to the database."""
        # TODO (use json config)
        for server in self._integration.stan.servers.values():
            await server.create_course_roles()
        return web.Response()
