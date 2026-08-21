from asqlite import connect, Connection
from asyncio import create_task, CancelledError, Task
from bot import Bot
from chat_ws_manager import ChatWSManager
from contextlib import asynccontextmanager
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.responses import FileResponse, HTMLResponse
from json import load
from os import makedirs
from os.path import abspath, dirname, join
from sqlite3 import Row
from twitchio import ChannelInfo, Client, User
from typing import AsyncGenerator
from uvicorn import run
from web_socket_manager import JsonElement

base_dir: str = dirname(abspath(__file__))
config_path: str = join(base_dir, "config.json")
chat_html_path: str = join(base_dir, "chat.html")
icon_path: str = join(base_dir, "logo.ico")

with open(config_path, "r") as f:
    config = load(f)


owner_login: str = config["OWNER_LOGIN"]
bot_login: str = config["BOT_LOGIN"]
client_id: str = config["CLIENT_ID"]
client_secret: str = config["CLIENT_SECRET"]
data_dir: str = config["DATA_DIR"]
api_port: int = config["API_PORT"]


bot: Bot
db: Connection
auth_cache: Connection
chat_ws_manager = ChatWSManager()


async def init_data() -> None:
    global db, auth_cache
    makedirs(data_dir, exist_ok=True)

    db = await connect(join(data_dir, "data.db"))
    async with db.cursor() as cursor:
        await db.execute(
            """
            CREATE TABLE IF NOT EXISTS feed_stats (
                id INTEGER PRIMARY KEY,
                last_feed_time INTEGER,
                current_count INTEGER,
                record_set_time INTEGER,
                record_count INTEGER,
                record_holder TEXT
            )
            """
        )
        await db.execute(
            "CREATE TABLE IF NOT EXISTS quotes (id INTEGER PRIMARY KEY, text TEXT, speaker TEXT, time INTEGER, quoter TEXT)"
        )
    await db.commit()

    auth_cache = await connect(join(data_dir, "auth_cache.db"))
    async with auth_cache.cursor() as cursor:
        await cursor.execute(
            "CREATE TABLE IF NOT EXISTS tokens (id INTEGER PRIMARY KEY, access TEXT, refresh TEXT)"
        )
        await cursor.execute(
            "CREATE TABLE IF NOT EXISTS user_ids (id INTEGER PRIMARY KEY, owner TEXT, bot TEXT)"
        )
    await auth_cache.commit()


async def fetch_user_ids() -> tuple[str, str]:
    row: Row | None = await auth_cache.fetchone("SELECT owner, bot FROM user_ids")
    if row:
        return (row["owner"], row["bot"])

    client = Client(client_id=client_id, client_secret=client_secret)
    await client.login()
    try:
        users: list[User] = await client.fetch_users(logins=[owner_login, bot_login])

        owner_id: str = next(
            u.id for u in users if str(u).lower() == owner_login.lower()
        )
        bot_id: str = next(u.id for u in users if str(u).lower() == bot_login.lower())

        await auth_cache.execute(
            """
            INSERT INTO user_ids
                (id, owner, bot)
            VALUES
                (?, ?, ?)
            ON CONFLICT(id) DO UPDATE SET
                owner = excluded.owner,
                bot = excluded.bot
            """,
            (1, owner_id, bot_id),
        )
        await auth_cache.commit()
        return (owner_id, bot_id)
    finally:
        await client.close()


async def fetch_tokens() -> tuple[str | None, str | None]:
    row: Row | None = await auth_cache.fetchone("SELECT access, refresh FROM tokens")
    if row:
        return (row["access"], row["refresh"])

    print("Please authorize at")
    print(
        "http://localhost:4343/oauth?scopes=user:read:chat+user:bot+user:write:chat&force_verify=true"
    )
    return (None, None)


@asynccontextmanager
async def lifespan(app: FastAPI) -> AsyncGenerator[None, None]:
    global bot

    await init_data()
    owner_id, bot_id = await fetch_user_ids()
    access_token, refresh_token = await fetch_tokens()

    bot = Bot(
        client_id=client_id,
        client_secret=client_secret,
        owner_id=owner_id,
        bot_id=bot_id,
        auth_cache=auth_cache,
        db=db,
        chat_ws_manager=chat_ws_manager,
    )

    if access_token and refresh_token:
        await bot.add_token(token=access_token, refresh=refresh_token)

    bot_task: Task[None] = create_task(bot.start(load_tokens=False, save_tokens=False))
    try:
        yield
    except CancelledError:
        pass
    finally:
        print()
        print("Shutting down")
        await bot.close()
        bot_task.cancel()
        await db.close()
        await auth_cache.close()


app = FastAPI(lifespan=lifespan)


@app.get("/favicon.ico", include_in_schema=False)
async def favicon() -> FileResponse:
    return FileResponse(icon_path)


@app.get("/chat", response_class=HTMLResponse)
async def chat() -> HTMLResponse:
    with open(chat_html_path, "r", encoding="utf-8") as f:
        return HTMLResponse(f.read())


@app.websocket("/chat/ws")
async def chat_websocket(websocket: WebSocket) -> None:
    await chat_ws_manager.connect(websocket)
    try:
        while True:
            await websocket.receive_text()
    except (WebSocketDisconnect, CancelledError):
        pass
    finally:
        chat_ws_manager.disconnect(websocket)


@app.get("/stream", response_model=JsonElement)
async def stream() -> JsonElement:
    channel_info: ChannelInfo | None = await bot.fetch_channel(bot.owner_id)
    if not channel_info:
        return None

    return {
        "classification_labels": [
            label for label in channel_info.classification_labels
        ],
        "delay": channel_info.delay,
        "game_id": channel_info.game_id,
        "game_name": channel_info.game_name,
        "is_branded_content": channel_info.is_branded_content,
        "language": channel_info.language,
        "tags": [tag for tag in channel_info.tags],
        "title": channel_info.title,
        "user": {
            "display_name": channel_info.user.display_name,
            "id": channel_info.user.id,
            "mention": channel_info.user.mention,
            "name": channel_info.user.name,
        },
    }


if __name__ == "__main__":
    run("main:app", host="127.0.0.1", port=api_port)
