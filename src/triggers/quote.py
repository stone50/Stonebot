from __future__ import annotations
from asqlite import Connection
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

    try:
        quote_id = int(matches[0])
    except ValueError:
        return

    row: Row | None = await db.fetchone(
        "SELECT text, speaker FROM quotes WHERE id = ?", (quote_id,)
    )
    if not row:
        return

    quote_text = row["text"]
    quote_speaker = row["speaker"]

    await message.respond(f'[{quote_id}] "{quote_text}" -{quote_speaker}')
