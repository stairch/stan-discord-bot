# -*- coding: utf-8 -*-
"""DB import web handler"""

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from aiohttp import web

from db.datamodels.hslu_student import HsluStudent
from .base_handler import BaseHandler


class DbImportHandler(BaseHandler):
    """Handler for importing students from CSV"""

    def _add_routes(self, app: web.Application) -> None:
        app.router.add_post("/api/students", self._upload_students)
        app.router.add_get("/api/students", self._get_students)

    async def _upload_students(self, request: web.Request) -> web.Response:
        """Upload students from a CSV file. These we'll write to the database."""
        plain = await request.text()
        new_graduates, new_students = self._db.update_students(
            HsluStudent.from_csv(plain)
        )
        for graduate in new_graduates:
            await self._stan.make_graduate(graduate)
        for student in new_students:
            await self._stan.make_student(student)
        return web.Response()

    async def _get_students(self, _: web.Request) -> web.Response:
        """Get number of students in the database."""
        return web.Response(text=str(len(self._db.all_students())))
