from __future__ import annotations
from asqlite import Connection
from random import randrange
from sqlite3 import Row
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    if not matches:
        return

    quote_id = 0
    try:
        quote_id = int(matches[0])
    except ValueError:
        rows: list[Row] = await db.fetchall("SELECT id FROM quotes")
        if not rows:
            return

        index: int = randrange(len(rows))
        row = rows[index]
        quote_id: int = row["id"]

    row: Row | None = await db.fetchone(
        "SELECT text, speaker FROM quotes WHERE id = ?", (quote_id,)
    )
    if not row:
        return

    quote_text: str = row["text"]
    quote_speaker: str = row["speaker"]

    await message.respond(f'[{quote_id}] "{quote_text}" -{quote_speaker}')
