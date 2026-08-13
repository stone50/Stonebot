# Stonebot
This is a chatbot for [Twitch](https://www.twitch.tv/).
 
## Setup

### Register an Application

You will need to register an application by following [these steps](https://dev.twitch.tv/docs/authentication/register-app/).

You will need the **Client ID** and **Client Secret** in the next step, so it is recommended to keep that information handy.

- Add a new **OAuth Redirect URL** and set it to `http://localhost:4343/oauth/callback`.

- Set the **Category** to `Chat Bot`.

- Set the **Client Type** to `Confidential`.

- Click **Create**

### Configure the Bot

In the same folder as the bot's entry point, create a file named `config.json`.

Here is what `config.json` should contain, using example values:
```
{
    "OWNER_LOGIN": "BroadcasterUsername",
    "BOT_LOGIN": "BotUsername",
    "CLIENT_ID": "<client id>",
    "CLIENT_SECRET": "<client secret>",
    "DATA_DIR": "/home/<user>/.local/share/Stonebot",
    "API_PORT": 4343
}
```

### Authorize

When you run the bot for the first time, it should print a URL to follow for authorization.

**Log in to the bot's Twitch account when authorizing.**

If you do want to authorize again (for example, if you used the wrong Twitch account), navigate to the `DATA_DIR` folder specified in `config.json`, and delete the `auth_cache.db` file.
When you run the bot again, it should prompt you to authorize again.
If you did use the wrong Twitch account to authorize, make sure to disconnect the application in that account's [settings](https://www.twitch.tv/settings/connections).

### BetterTTV

Some of the chat messages sent by the bot include emotes from [BetterTTV](https://betterttv.com/).
Anyone who wants to properly view these emotes will need to have BetterTTV installed in their browser.
The broadcaster will need to have the following emotes added to their channel:

![BARF2](https://cdn.betterttv.net/emote/5f9e991d58e96102e92a76f1/1x.webp) [BARF2](https://betterttv.com/emotes/5f9e991d58e96102e92a76f1) by [SHXK3Y](https://betterttv.com/users/5de74567e7df1277b606d33c)

![BARF3](https://cdn.betterttv.net/emote/5f9e992f1b017902db156fd8/1x.webp) [BARF3](https://betterttv.com/emotes/5f9e992f1b017902db156fd8) by [SHXK3Y](https://betterttv.com/users/5de74567e7df1277b606d33c)

![crayonTime](https://cdn.betterttv.net/emote/5c6ab708adab351034b4050f/1x.webp) [crayonTime](https://betterttv.com/emotes/5c6ab708adab351034b4050f) by [NymN](https://betterttv.com/users/559ad81fa287f9ec6c0a6ff3)

![popCat](https://cdn.betterttv.net/emote/5fa8f232eca18f6455c2b2e1/1x.webp) [popCat](https://betterttv.com/emotes/5fa8f232eca18f6455c2b2e1) by [EthynWithAY](https://betterttv.com/users/5b458a6b9733463289f1408e)
