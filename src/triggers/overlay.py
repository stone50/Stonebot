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
        "Levels with red backgrounds require more skilled execution. Levels with blue text are highly reliant on RNG. | chat: github.com/stone50/Stonebot | inputs: github.com/stone50/Input-Overlay | splits: github.com/stone50/WYS-Blindfolded-Splitter-Overlay"
    )
