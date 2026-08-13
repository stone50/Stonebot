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

    row: Row | None = await db.fetchone(
        "SELECT record_count, record_holder FROM feed_stats"
    )

    if not row:
        await message.respond("Feed the cat first...")
        return

    record = row["record_count"]
    holder = row["record_holder"]

    await message.respond(f"The record is {record}, last fed by {holder}")
