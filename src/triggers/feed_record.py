from __future__ import annotations
from asqlite import Connection
from sqlite3 import Row
from time import time
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


def get_ago_text(record_set_time: int) -> str:
    seconds_since_record: int = int(time()) - record_set_time

    years_since_record: int = int(seconds_since_record / (60 * 60 * 24 * 30 * 12))
    if years_since_record:
        return f"{years_since_record} year{'s' if years_since_record > 1 else ''} ago"

    months_since_record: int = int(seconds_since_record / (60 * 60 * 24 * 30))
    if months_since_record:
        return (
            f"{months_since_record} month{'s' if months_since_record > 1 else ''} ago"
        )

    days_since_record: int = int(seconds_since_record / (60 * 60 * 24))
    if days_since_record:
        return f"{days_since_record} day{'s' if days_since_record > 1 else ''} ago"

    hours_since_record: int = int(seconds_since_record / (60 * 60))
    if hours_since_record:
        return f"{hours_since_record} hour{'s' if hours_since_record > 1 else ''} ago"

    minutes_since_record: int = int(seconds_since_record / 60)
    if minutes_since_record:
        return f"{minutes_since_record} minute{'s' if minutes_since_record > 1 else ''} ago"

    return f"{seconds_since_record} second{'s' if seconds_since_record > 1 else ''} ago"


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:

    row: Row | None = await db.fetchone(
        "SELECT record_set_time, record_count, record_holder FROM feed_stats"
    )

    if not row:
        await message.respond("Feed the cat first...")
        return

    record_set_time: int = row["record_set_time"]
    record_count: int = row["record_count"]
    record_holder: str = row["record_holder"]

    ago_text: str = get_ago_text(record_set_time)
    await message.respond(f"The record is {record_count}, fed {ago_text} by {record_holder}")
