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

    overlapping_matches: tuple[str, str] = matches[0]
    if len(overlapping_matches) != 2:
        return

    match_quote_id, new_text = overlapping_matches

    try:
        quote_id = int(match_quote_id)
    except ValueError:
        return

    row: Row | None = await db.fetchone(
        "SELECT id FROM quotes WHERE id = ?", (quote_id,)
    )
    if not row:
        return

    await db.execute("UPDATE quotes SET text = ? WHERE id = ?", (new_text, quote_id))
    await db.commit()

    await message.respond(f"Quote {quote_id} updated")
