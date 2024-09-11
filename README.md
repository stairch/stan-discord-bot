# Stan Discord Bot

This repository contains version 3 of the STAIR Discord bot, Stan.

## Table of Contents

-   [Introduction](#introduction)
-   [Components](#components)
    -   [Web Server](#web-server)
    -   [Integration](#integration)
    -   [Database](#database)
    -   [UI](#ui)
-   [Setup and Installation](#setup-and-installation)
-   [CI/CD](#cicd)

## Introduction

Stan is a Discord bot designed to help students at the University of Applied Sciences and Arts Lucerne (HSLU) with various tasks. It provides features such as:

-   Announcement management
-   Foodstoffi menu notifications
-   Discord student verification

## Components

### Web Server

The web server is the main entry point for the STAIR PR Toolkit. It handles incoming requests and routes them to the appropriate handlers.

-   **Main Entry Point**: [`service/webserver/server.py`](/service/webserver/server.py)
-   **Handlers**:
    -   [`DbImportHandler`](/service/webserver/db_import.py)
    -   [`AnnouncementHandler`](/service/webserver/announcement.py)
    -   [`FoodstoffMenuTrigger`](/service/webserver/foodstoffi_menu_trigger.py)
    -   [`MsalAuth`](/service/webserver/msal_auth.py)

### Integration

The integration module manages external services and APIs.

-   **Main Manager**: [`IntegrationManager`](/service/integration/manager.py)
-   **Telegram Announcer**: [`service/integration/telegram/announcer.py`](/service/integration/telegram/announcer.py)
-   **Discord Announcer**: [`service/integration/discord/announcer.py`](/service/integration/discord/announcer.py)
-   **Foodstoffi Menu**: [`service/integration/foodstoffi/menu.py`](/service/integration/foodstoffi/menu.py)
-   **Email (via MSGraph)**: [`service/integration/email/client.py`](/service/integration/email/client.py)

### Database

The database module handles all interactions with the database, which stores discord users, announcements, and other data.
PostgreSQL is used as the database system, but the dataset python library is used to interact with the database.

-   **Database Class**: [`Database`](/service/db/db.py)

### UI

The UI module contains the frontend code for the application. It is built with Vue.JS 3.

-   **Main Entry Point**: [`ui/index.html`](/ui/index.html)
-   **Source Code**: [`ui/src/`](/ui/src/)

## Setup and Installation

To set up and run the project, follow these steps:

1. **Clone the repository**:

    ```sh
    git clone <repository-url>
    cd <repository-directory>
    ```

2. **Set up environment variables**:
   Create a [`.env`](/.env) file in the root directory and add the necessary environment variables:

    ```sh
    # Azure AD (for sending emails and for the web app authentication)
    AD_APP_ID=""
    AD_APP_SECRET_ID=""
    AD_APP_SECRET=""
    AD_TENANT_ID=""

    # Email
    EMAIL_ADDRESS="" # email address to be used (e.g., for the 2FA mail)

    # Discord
    DISCORD_APP_ID="1234567890" # discord application ID (from discord developer portal)
    DISCORD_SERVERS="12345,54321" # comma-separated list of allowed discord server IDs

    # Telegram
    TELEGRAM_BOT_TOKEN="1234567890:ABCDEF" # telegram bot token (from BotFather)
    TELEGRAM_CHATS="12345,54321" # comma-separated list of allowed telegram group chat IDs

    # Database
    POSTGRES_PASSWORD="" # database password
    POSTGRES_USER="" # database user
    POSTGRES_DB="" # database name

    # Web Server
    SESSION_SECRET="" # random string, used for the session cookie (web app)

    # General
    DEV_MODE="False" # when true, the database will be cleared on startup
    ```

3. **Build and run the Docker containers**:

    ```sh
    docker-compose up --build
    ```

4. **Install frontend dependencies**:

    ```sh
    cd ui
    npm install
    npm run dev
    ```

5. **Access the application**:
   Open your browser and navigate to `http://localhost:5173`.

For more detailed instructions, refer to the individual README files in the [`service`](/service) and [`ui`](/ui) directories.

## CI/CD

The project uses GitHub Actions for CI/CD. The workflows are defined in the [`.github/workflows`](/.github/workflows) directory.
Currently, only the server-side (Python) code is tested with the CI/CD pipeline.

You can run the checks locally using the following command:

```sh
cd service
python3 -m pip install mypy pylint ruff
./lint.sh # this will also reformat the code using ruff
```
