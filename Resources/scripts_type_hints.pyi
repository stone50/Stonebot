from typing import List, Optional, Any


class ChatResponse:
    data: List["ChatResponse.DataPoint"]
    drop_reason: "ChatResponse.DropReason"

    class DataPoint:
        message_id: str
        is_sent: bool

    class DropReason:
        code: str
        message: str


class ChatterPermission:
    level: int
    role: str


class MessageData:
    broadcaster_id: str
    broadcaster_user_name: str
    broadcaster_login: str
    chatter_id: str
    chatter_user_name: str
    chatter_login: str
    message_id: str
    message: "MessageData.EventMessage"
    message_type: str
    badges: List["MessageData.EventBadge"]
    cheer: Optional["MessageData.EventCheer"]
    color: str
    reply: Optional["MessageData.EventReply"]
    channel_points_custom_reward_id: Optional[str]
    source_broadcaster_id: Optional[str]
    source_broadcaster_user_name: Optional[str]
    source_broadcaster_login: Optional[str]
    source_message_id: Optional[str]
    source_badges: Optional["MessageData.EventSourceBadges"]
    is_source_only: Optional[bool]

    class EventMessage:
        text: str
        fragments: List["MessageData.EventMessageFragment"]

    class EventMessageFragment:
        type: str
        text: str
        cheermote: Optional["MessageData.EventMessageFragmentCheermote"]
        emote: Optional["MessageData.EventMessageFragmentEmote"]
        mention: Optional["MessageData.EventMessageFragmentMention"]

    class EventMessageFragmentCheermote:
        prefix: str
        bits: int
        tier: int

    class EventMessageFragmentEmote:
        id: str
        emote_set_id: str
        owner_id: str
        format: List[str]

    class EventMessageFragmentMention:
        user_id: str
        user_name: str
        user_login: str

    class EventBadge:
        set_id: str
        badge_id: str
        info: str

    class EventCheer:
        bits: int

    class EventReply:
        parent_message_id: str
        parent_message_body: str
        parent_user_id: str
        parent_user_name: str
        parent_user_login: str
        thread_message_id: str
        thread_user_id: str
        thread_user_name: str
        thread_user_login: str

    class EventSourceBadges:
        set_id: str
        badge_id: str
        info: str


class ScriptInterface:
    message_data: MessageData
    chatter_permission: ChatterPermission

    @staticmethod
    def log(*messages: Any) -> None: ...

    @staticmethod
    def log_warn(*messages: Any) -> None: ...

    @staticmethod
    def log_error(*messages: Any) -> None: ...

    @staticmethod
    def chat(message: str, reply_parent_message_id: Optional[str] = None) -> ChatResponse: ...

    @staticmethod
    def get_data(key: str) -> Any: ...

    @staticmethod
    def set_data(key: str, value: Any) -> None: ...

    @staticmethod
    def set_data_without_saving(key: str, value: Any) -> None: ...

    @staticmethod
    def delete_data(key: str) -> bool: ...

    @staticmethod
    def delete_data_without_saving(key: str) -> bool: ...

    @staticmethod
    def data_contains(key: str) -> bool: ...

    @staticmethod
    def save_data() -> None: ...

Stonebot: ScriptInterface