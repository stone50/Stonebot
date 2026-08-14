from __future__ import annotations
from asqlite import Connection
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    await message.respond("github.com/stone50/Stonebot/blob/main/triggers.md")
