reset
python3 -m mypy .
python3 -m ruff check --fix .
python3 -m ruff format .
python3 -m pylint *.py */ --recursive=y --include-naming-hint=y