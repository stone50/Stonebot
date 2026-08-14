from asqlite import Connection
from collections.abc import Awaitable, Callable
from re import compile, findall, Pattern
from twitchio.types_ import TokenMappingData
from twitchio import ChatMessage, ChatMessageDelete, Client, TokenRefreshedPayload
from twitchio.authentication import UserTokenPayload
from twitchio.eventsub import ChatMessageSubscription, ChatMessageDeleteSubscription
from typing import Any

from triggers import (
    add_quote,
    ask,
    delete_quote,
    discord,
    edit_quote,
    feed_record,
    feed,
    lurk,
    quote,
    youtube,
)


class Bot(Client):
    def __init__(
        self,
        client_id: str,
        client_secret: str,
        owner_id: str,
        bot_id: str,
        db: Connection,
        auth_cache: Connection,
    ) -> None:
        super().__init__(
            client_id=client_id, client_secret=client_secret, bot_id=bot_id
        )

        self._owner_id: str = owner_id
        self._db: Connection = db
        self._auth_cache: Connection = auth_cache
        self._triggers: list[
            tuple[
                Pattern[str],
                Callable[[ChatMessage, list[Any], Bot, Connection], Awaitable[None]],
            ]
        ] = [
            (compile(r"^!addquote (.+)"), add_quote.run),
            (compile(r"^!ask\b(.*)"), ask.run),
            (compile(r"^!deletequote ([0-9]+)"), delete_quote.run),
            (compile(r"^!(discord|dc)\b"), discord.run),
            (compile(r"^!editquote ([0-9]+) (.+)"), edit_quote.run),
            (compile(r"^!feedrecord\b"), feed_record.run),
            (compile(r"^!feed\b"), feed.run),
            (compile(r"^!lurk\b"), lurk.run),
            (compile(r"^!quote ([0-9]+)"), quote.run),
            (compile(r"^!(youtube|yt)\b"), youtube.run),
        ]

    def owner_id(self) -> str:
        return self._owner_id

    async def reply(self, message: str, reply_to_message: ChatMessage) -> None:
        await reply_to_message.broadcaster.send_message(
            message, sender=str(self.bot_id), reply_to_message_id=reply_to_message.id
        )

    async def event_ready(self) -> None:
        if self.tokens:
            await self._subscribe()

    async def event_message(self, payload: ChatMessage) -> None:
        print(f"{payload.chatter}: {payload.text}")

        if payload.chatter.id != self.bot_id:
            for pattern, run in self._triggers:
                matches: list[Any] = findall(pattern, payload.text)
                if matches:
                    await run(payload, matches, self, self._db)

    async def event_message_delete(self, payload: ChatMessageDelete) -> None:
        print(f"Message {payload.message_id} deleted")

    async def save_tokens(self, path: str | None = None) -> None:
        token_data: TokenMappingData = next(iter(self.tokens.values()))
        access_token: str = token_data.get("token")
        refresh_token: str = token_data.get("refresh")
        await self._auth_cache.execute(
            """
            INSERT INTO tokens
                (id, access, refresh)
            VALUES
                (?, ?, ?)
            ON CONFLICT(id) DO UPDATE SET
                access = excluded.access,
                refresh = excluded.refresh
            """,
            (1, access_token, refresh_token),
        )
        await self._auth_cache.commit()

    async def event_oauth_authorized(self, payload: UserTokenPayload) -> None:
        await super().event_oauth_authorized(payload)
        await self.save_tokens()
        await self._subscribe()

    async def event_token_refreshed(self, payload: TokenRefreshedPayload) -> None:
        await self.save_tokens()

    async def _subscribe(self) -> None:
        await self.subscribe_websocket(
            ChatMessageSubscription(
                broadcaster_user_id=self._owner_id, user_id=self.bot_id
            ),
            as_bot=True,
        )
        await self.subscribe_websocket(
            ChatMessageDeleteSubscription(
                broadcaster_user_id=self._owner_id, user_id=self.bot_id
            ),
            as_bot=True,
        )
