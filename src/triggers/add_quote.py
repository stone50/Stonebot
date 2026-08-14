from __future__ import annotations
from asqlite import Connection
from sqlite3 import Row
from time import time
from twitchio import ChatMessage, Chatter
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    chatter: Chatter = message.chatter
    if not (message.chatter.vip or chatter.moderator or chatter.broadcaster):
        return

    if not matches:
        return

    row: Row = await db.fetchone(
        "INSERT INTO quotes (text, speaker, time, quoter) VALUES (?, ?, ?, ?) RETURNING id",
        (matches[0], str(message.broadcaster), int(time()), str(chatter)),
    )
    await db.commit()

    quote_id = row["id"]

    await message.respond(f"Quote {quote_id} added")
