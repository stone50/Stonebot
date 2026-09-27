from __future__ import annotations
from asqlite import Connection
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    await message.respond(
        "Chapter completion estimates: A=10 mins, B=10 mins, C=20-30 mins, D=10 mins, E=TBD | strats: https://docs.google.com/document/d/1JleYdtU9Syj-5FnZQXraqlgZryAg4jnWdIdapbf5Q1E/edit?usp=sharing | spliced run: https://youtu.be/HyZNrisg4uU"
    )
