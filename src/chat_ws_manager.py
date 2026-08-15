from twitchio import ChatMessage
from web_socket_manager import JsonElement, WebSocketManager


class ChatWSManager(WebSocketManager):
    async def broadcast_new_message(self, message: ChatMessage) -> None:
        payload_fragments: list[JsonElement] = []
        for fragment in message.fragments:
            type = "text"
            text = ""
            if fragment.cheermote:
                text: str = fragment.cheermote.prefix + str(fragment.cheermote.bits)
            elif fragment.emote:
                type = "image"
                text = f"https://static-cdn.jtvnw.net/emoticons/v2/{fragment.emote.id}/default/dark/1.0"
            elif fragment.gif:
                type = "image"
                text = fragment.gif.url
            elif fragment.mention:
                text = fragment.text
            elif fragment.text:
                text = fragment.text

            payload_fragments.append({"type": type, "text": text})

        await self.broadcast(
            {
                "event": "new_message",
                "id": message.id,
                "chatter_name": message.chatter.display_name,
                "chatter_color": (
                    message.chatter.colour.html if message.chatter.colour else "#FF6900"
                ),
                "fragments": payload_fragments,
            }
        )

    async def broadcast_delete_message(self, id: str) -> None:
        await self.broadcast({"event": "delete_message", "id": id})
