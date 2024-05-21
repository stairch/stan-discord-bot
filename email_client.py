# -*- coding: utf-8 -*-
"""Email client module"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

import os
import logging
from msal import PublicClientApplication  # type: ignore
from dotenv import load_dotenv
import aiohttp

load_dotenv()

AD_APP_ID = os.getenv("AD_APP_ID")
EMAIL_ADDRESS = os.getenv("EMAIL_ADDRESS")
EMAIL_NAME = os.getenv("EMAIL_NAME")
SCOPES = ["Mail.Send"]


class EmailClient:
    """Client for sending emails via Microsoft Graph API."""

    def __init__(self) -> None:
        self._logger = logging.getLogger(__name__)
        self._app = PublicClientApplication(
            AD_APP_ID, authority="https://login.microsoftonline.com/common"
        )
        flow = self._app.initiate_device_flow(SCOPES)
        self._logger.info(flow["message"])
        self._app.acquire_token_by_device_flow(flow)
        accounts = self._app.get_accounts()
        assert len(accounts) > 0
        self._account = accounts[0]

    def _get_token(self) -> str:
        result = self._app.acquire_token_silent(SCOPES, account=self._account)
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
                "https://graph.microsoft.com/v1.0/me/sendMail",
                headers={"Authorization": "Bearer " + self._get_token()},
                json=body,
            ) as r:
                return r.status == 202
