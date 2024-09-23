reset
python3 -m mypy .
ruff check --fix .
ruff format .
python3 -m pylint *.py */ --recursive=y --include-naming-hint=y