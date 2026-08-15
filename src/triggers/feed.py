from __future__ import annotations
from asqlite import Connection
from random import randrange
from sqlite3 import Row
from time import time
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    now: int = int(time())

    row: Row | None = await db.fetchone(
        "SELECT last_feed_time, current_count, record_set_time, record_count, record_holder FROM feed_stats"
    )

    last_feed_time: int = row["last_feed_time"] if row else 0
    current_count: int = row["current_count"] if row else 0
    record_set_time: int = row["record_set_time"] if row else 0
    record_count: int = row["record_count"] if row else 0
    record_holder: str = row["record_holder"] if row else ""

    if current_count and randrange(max(1, min((now - last_feed_time) * 2, 100))) == 0:
        current_count = 0
        await message.respond(
            f"popCat BARF2 BARF3 {message.chatter}, you fed the cat too fast!"
        )
    else:
        current_count += 1
        if current_count > record_count:
            record_set_time = now
            record_count = current_count
            record_holder = str(message.chatter)

        await message.respond(f"popCat crayonTime x{current_count}")

    await db.execute(
        """
        INSERT INTO feed_stats
            (id, last_feed_time, current_count, record_set_time, record_count, record_holder)
        VALUES
            (?, ?, ?, ?, ?)
        ON CONFLICT(id) DO UPDATE SET
            last_feed_time = excluded.last_feed_time,
            current_count = excluded.current_count,
            record_set_time = excluded.record_set_time,
            record_count = excluded.record_count,
            record_holder = excluded.record_holder
    """,
        (1, now, current_count, record_set_time, record_count, record_holder),
    )
    await db.commit()
