from __future__ import annotations
from asqlite import Connection
from sqlite3 import Row
from twitchio import ChatMessage, Chatter
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    chatter: Chatter = message.chatter
    if not chatter.moderator or not chatter.broadcaster:
        return

    if not matches:
        return

    try:
        quote_id = int(matches[0])
    except ValueError:
        return

    row: Row | None = await db.fetchone(
        "SELECT id FROM quotes WHERE id = ?", (quote_id,)
    )
    if not row:
        return

    await db.execute("DELETE FROM quotes WHERE id = ?", (quote_id,))
    await db.commit()

    await message.respond(f"R.I.P quote {quote_id}")
