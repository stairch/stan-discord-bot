# -*- coding: utf-8 -*-
"""Foodstoffi Menu Scraper"""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from dataclasses import dataclass, field
import re
import datetime
import logging

import aiohttp
import discord
import bs4
from pyaddict import JDict, JList

from common.aioschedule import AioSchedule
from common.constants import STAIR_GREEN
from db.datamodels.announcement import AnnouncementType
from integration.discord.stan import Stan
from integration.discord.persona import PersonaSender, Personas
from integration.discord.server import AnnouncementChannelType

URL = "https://app.food2050.ch/de/foodstoffi/foodstoffi/menu/foodstoffi/weekly"


@dataclass
class Recipe:  # pylint: disable=too-many-instance-attributes
    """A recipe"""

    typename: str
    id: str
    category: str
    is_balanced: bool
    title: str
    climate_prediction: str
    slug: str
    is_vegan: bool
    is_vegetarian: bool
    _allergens: list[str] = field(default_factory=list)

    @classmethod
    def from_dict(cls, value: JDict, category: str) -> Recipe | None:
        """Create a recipe from a dictionary"""
        if not value:
            return None

        if "Geschlossen" in value.ensure("title", str):
            return None

        return cls(
            typename=value.ensure("__typename", str),
            id=value.ensure("id", str),
            category=category,
            is_balanced=value.ensure("isBalanced", bool),
            title=value.ensure("title", str),
            climate_prediction=value.ensureCast("climatePrediction", JDict)
            .ensure("rating", str)
            .lower(),
            slug=value.ensure("slug", str),
            _allergens=value.ensure("allergens", str).split(","),
            is_vegan=value.ensure("isVegan", bool),
            is_vegetarian=value.ensure("isVegetarian", bool),
        )

    @property
    def allergens(self) -> list[str]:
        """Get the allergens"""
        return [
            re.sub(r"(?<=[a-z])(?=[A-Z])", " ", allergen).title()
            for allergen in self._allergens
            if allergen != ""
        ]

    @property
    def as_embed(self) -> discord.Embed:
        """Get the recipe as an embed"""
        embed = discord.Embed(
            title=self.category,
            url=f"https://app.food2050.ch/de/foodstoffi/foodstoffi/food-profile/{self.slug}",
            description=self.title,
            color=STAIR_GREEN,
        )
        tags: list[str] = []
        if self.is_vegan:
            tags.append("🌱 Vegan")
        elif self.is_vegetarian:
            tags.append("🌿 Vegetarian")
        if self.is_balanced:
            tags.append("⚖️ Balanced")
        if self.climate_prediction:
            tags.append(f" 🌍 {self.climate_prediction.title()} Climate Impact")
        if self.allergens:
            tags.append(f"🚫 {', '.join(self.allergens)}")
        embed.set_footer(text=" | ".join(tags))
        return embed


@dataclass
class RecipeItem:
    """A recipe item"""

    typename: str
    date: datetime.date
    recipe: Recipe | None

    @classmethod
    def from_dict(cls, value: JDict, category: str) -> RecipeItem | None:
        """Create a recipe item from a dictionary"""
        item = cls(
            typename=value.ensure("__typename", str),
            date=datetime.datetime.fromisoformat(value.ensure("date", str)).date()
            + datetime.timedelta(days=1),
            recipe=Recipe.from_dict(value.ensureCast("recipe", JDict), category),
        )
        if not item.recipe:
            return None
        return item


@dataclass
class Category:
    """A category of recipes"""

    typename: str
    id: str
    name: str
    recipes: list[RecipeItem]

    @classmethod
    def from_dict(cls, value: JDict) -> Category:
        """Create a category from a dictionary"""
        name = value.ensure("displayName", str)
        recipes = [
            RecipeItem.from_dict(recipe, name)
            for recipe in value.ensureCast("dailyRecipies", JList)
            .iterator()
            .ensureCast(JDict)
        ]
        recipes = [recipe for recipe in recipes if recipe]
        return cls(
            typename=value.ensure("__typename", str),
            id=value.ensure("id", str),
            name=name,
            recipes=recipes,  # type: ignore
        )

    @property
    def todays_recipe(self) -> Recipe | None:
        """Get the recipe for today"""
        today = datetime.date.today()
        for recipe in self.recipes:
            if recipe.date == today:
                return recipe.recipe
        return None


@dataclass
class Menu:
    """A menu"""

    typename: str
    id: str
    note: str
    categories: list[Category]

    @classmethod
    def _from_dict(cls, value: JDict) -> Menu:
        return cls(
            typename=value.ensure("__typename", str),
            id=value.ensure("id", str),
            note=value.ensure("note", str),
            categories=[
                Category.from_dict(category)
                for category in value.ensureCast("categories", JList)
                .ensureCast(0, JDict)
                .ensureCast("items", JList)
                .iterator()
                .ensureCast(JDict)
            ],
        )

    @property
    def today_recipes(self) -> list[Recipe]:
        """Get the recipes for today"""
        return [
            category.todays_recipe
            for category in self.categories
            if category.todays_recipe
        ]

    @staticmethod
    async def get_todays_menu() -> list[Recipe] | None:
        """Get the menu for today"""
        async with aiohttp.ClientSession() as session:
            async with session.get(URL) as response:
                html = await response.text()
                soup = bs4.BeautifulSoup(html, "html.parser")
                tag = soup.find("script", {"id": "__NEXT_DATA__"})
                if not isinstance(tag, bs4.Tag):
                    return None
                props = JDict.fromString(str(tag.string)).chain()
        raw_menu = props.ensureCast(
            "props.pageProps.query.location.kitchen.digitalMenu", JDict
        )
        menu = Menu._from_dict(raw_menu)
        today_recipes = menu.today_recipes
        if not today_recipes:
            return None
        if any("ferien" in x.title.lower() for x in today_recipes):
            return None
        return today_recipes


class SendFoodstoffiMenuTask:
    """Task to send the Foodstoffi menu"""

    def __init__(self, discord_bot: Stan) -> None:
        self._logger = logging.getLogger("FoodstoffiMenu")
        self._discord_bot = discord_bot

    async def start(self) -> None:
        """Start the task"""
        AioSchedule.run_daily_at(
            datetime.time(hour=8, minute=0),  # in UTC
            self.trigger,
        )

    async def trigger(self) -> None:
        """Send a foodstoffi menu update to all servers"""
        todays_menu = await Menu.get_todays_menu()
        if todays_menu is None:
            self._logger.warning("No menu available")
            return
        for server in self._discord_bot.servers.values():
            channel_type = AnnouncementChannelType.from_announcement_type(
                AnnouncementType.CANTEEN_MENU
            )
            channel = channel_type.get(server.guild)

            if channel is None:
                self._logger.warning("No channel found for server %s", server.guild)
                continue

            role = channel_type.get_role(server.guild)

            await PersonaSender(channel, Personas.CHEF).send(
                f"Hiya, {role.mention}! This is today's menu:",
                [x.as_embed for x in todays_menu],
            )
