# -*- coding: utf-8 -*-
"""Data model for a student at the HSLU."""

from __future__ import annotations

__copyright__ = "Copyright (c) 2024 STAIR. All Rights Reserved."
__email__ = "info@stair.ch"

from typing import Any
from dataclasses import dataclass
from enum import StrEnum
import csv
from pyaddict import JDict


class StudyModel(StrEnum):
    """The study model of the student"""

    FULL_TIME = "Vollzeitstudium"
    PART_TIME = "Teilzeitstudium"
    UNKNOWN = "Unknown"

    @classmethod
    def get(cls, value: str) -> StudyModel:
        """Get the study model from a string"""
        try:
            return cls(value)
        except ValueError:
            return cls.UNKNOWN


@dataclass
class HsluStudent:  # pylint: disable=too-many-instance-attributes
    """Data model for a student at the HSLU, as provided by HSLU."""

    id: int
    first_name: str
    last_name: str
    course_id: str  # e.g., I.BSCAIML.2001
    course_name: (
        str  # e.g., I.Bachelor of Science in Artificial Intelligence & Machine Learning
    )
    status: str  # eg., jA.Immatrikuliert (whatever that's supposed to mean)
    start_date: str  # e.g., 14.09.2020
    study_model: StudyModel
    email: str  # e.g., a.b@stud.hslu.ch

    @classmethod
    def _from_csv_dict(cls, value: dict[str, Any]) -> HsluStudent:
        data = JDict(value)
        return cls(
            data.ensureCast("ID Person", int),
            data.ensure("Vornamen", str),
            data.ensure("Nachname", str),
            data.ensure("Anlassnummer", str),
            data.ensure("Anlassbezeichnung", str),
            data.ensure("Status (Anmeldung)", str),
            data.ensure("Eintritt per", str),
            StudyModel.get(data.ensure("Ausbildungsform", str)),
            data.ensure("email", str),
        )

    @classmethod
    def from_csv(cls, plain: str) -> list[HsluStudent]:
        """Create a list of students from a CSV document"""
        plain = plain.replace("E-Mail", "email")
        students = csv.DictReader(plain.splitlines())
        return [cls._from_csv_dict(student) for student in students]
