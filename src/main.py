from asyncio import CancelledError
from bot import Bot
from asqlite import connect, Connection
from asyncio import create_task, Task
from contextlib import asynccontextmanager
from fastapi import FastAPI
from json import load
from os import makedirs
from os.path import abspath, dirname, join
from sqlite3 import Row
from twitchio import Client, User
from typing import AsyncGenerator
from uvicorn import run

with open(join(dirname(abspath(__file__)), "config.json"), "r") as f:
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


async def init_data() -> None:
    global db, auth_cache
    makedirs(data_dir, exist_ok=True)

    db = await connect(join(data_dir, "data.db"))
    async with db.cursor() as cursor:
        await db.execute(
            "CREATE TABLE IF NOT EXISTS feed_stats (id INTEGER PRIMARY KEY, last_feed_time INTEGER, current_count INTEGER, record_count INTEGER, record_holder TEXT)"
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


@app.get("/chat")
async def chat() -> None:
    pass


if __name__ == "__main__":
    run("main:app", host="127.0.0.1", port=api_port)
