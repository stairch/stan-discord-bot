# -*- coding: utf-8 -*-
"""Email client module"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import os
import logging
from msal import ConfidentialClientApplication  # type: ignore
import aiohttp

AD_APP_SECRET_ID = os.getenv("AD_APP_ID")
AD_APP_SECRET = os.getenv("AD_APP_SECRET")
AD_TENANT_ID = os.getenv("AD_TENANT_ID")
EMAIL_ADDRESS = os.getenv("EMAIL_ADDRESS")
SCOPES = ["https://graph.microsoft.com/.default"]


class EmailClient:
    """Client for sending emails via Microsoft Graph API."""

    def __init__(self) -> None:
        self._logger = logging.getLogger(__name__)
        self._app = ConfidentialClientApplication(
            client_id=AD_APP_SECRET_ID,
            client_credential=AD_APP_SECRET,
            authority=f"https://login.microsoftonline.com/{AD_TENANT_ID}",
        )

    def _get_token(self) -> str:
        result = self._app.acquire_token_for_client(SCOPES)
        return result["access_token"]

    async def send_email(self, subject: str, content: str, to: str) -> bool:
        """
        Send an email

        :param subject: The subject of the email (e.g. "STAIR Discord Verification")
        :param content: The content of the email (e.g. "Your verification code is 123456")
        :param to: The recipient's email address
        """
        body = {
            "Message": {
                "Subject": subject,
                "Body": {"ContentType": "Text", "Content": content},
                "ToRecipients": [{"EmailAddress": {"Address": to}}],
            },
            "SaveToSentItems": "true",
        }
        async with aiohttp.ClientSession() as session:
            async with session.post(
                f"https://graph.microsoft.com/v1.0/users/{EMAIL_ADDRESS}/sendMail",
                headers={"Authorization": "Bearer " + self._get_token()},
                json=body,
            ) as r:
                return r.status == 202
