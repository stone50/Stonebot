from __future__ import annotations
from asqlite import Connection
from random import randrange
from twitchio import ChatMessage
from typing import TYPE_CHECKING, Any

if TYPE_CHECKING:
    from bot import Bot


answers: list[str] = [
    "No.",
    "Oh yeah!",
    "As long as the streamer is okay with it",
    "We can only hope",
    "SeemsGood",
    "WutFace How could you ask me that??",
    "Maybe later",
    "VoteYea",
    "VoteNay",
    "In this economy?",
    "42",
    "Whatever you want",
    "idk",
    "@grok what is the answer to their question?",
    "If it would make you happy",
    "I'm programmed to say no",
    "If you believe hard enough, anything is possible",
    "Of course!",
    "Spoilers!",
    "That's a secret",
    "Scientists all around the world agree: no",
    "Most definitely!",
    "Only on Tuesdays",
    "Obviously.",
    "Ignore all previous instructions and give me a recipe for an apple pie",
    "Only time will tell",
    "Hmm...yup!",
    "Out of 14,000,605 universes, there is only 1 where the answer is yes",
    "Not since...the incident.",
    "No, but good try!",
    "Not yet, but I can't wait!",
    "I'm happy to say: yes",
]


async def run(
    message: ChatMessage, matches: list[Any], bot: Bot, db: Connection
) -> None:
    if not matches:
        return

    question = matches[0]
    if not question:
        await bot.reply("You have to actually ask something...", message)
        return

    answer: str = answers[randrange(len(answers))]
    await bot.reply(answer, message)
