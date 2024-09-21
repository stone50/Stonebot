# Stone Bot
This is a chatbot for [Twitch](https://www.twitch.tv/).
 
## Setup

### Setup Twitch Accounts

This bot is designed to use a separate Twitch account to send chat messages to a broadcaster.
It is possible to use the broadcaster's account, but the bot will still behave as if it is two different accounts.

You will need to be able to login to both the bot's account and the broadcaster's account.

### Register a Chatter and Collector Application

Using any Twitch account, you will need to register two applications by following [these steps](https://dev.twitch.tv/docs/authentication/register-app/).

You will need the Client ID and Client Secret for both applications in the next step, so it is recommended to keep the browser tab(s) open until then.

One of the applications will be used to send chat messages, and the other will be used to collect data from the broadcaster.
It is recommended you name the two applications accordingly (ex. "BenBot Chatter" and "BenBot Collector").

For both applications:

-Set a redirect URL to localhost along with a port (ex. "http://localhost:6969").
Both applications need to have the same redirect URL.

-Set the category to "Chat Bot".

-Set the Client Type to "Confidential".

### Configure the Bot

Add a `config.json` file to the project folder:
```
{
  "authorizationPort": 6969,
  "chatterClientId": <your chatter application client id>,
  "chatterClientSecret": <your chatter application client secret>,
  "chatterScope": [ "user:write:chat" ],
  "collectorClientId": <your collector application client id>,
  "collectorClientSecret": <your collector client secret>,
  "collectorScope": [ "user:read:chat", "channel:read:vips", "channel:read:subscriptions", "moderation:read" ],
  "socketKeepaliveBuffer": 5000,
  "socketKeepaliveTimeout": 10,
  "tokenExpirationBuffer": 1000
}
```
-`authorizationPort` [Integer]
This is the localhost port that will be used when you authenticate the bot.
This needs to be the same port that is specified in the "OAuth Redirect URLs" field in the [Twitch Console](https://dev.twitch.tv/console/apps).
For example, if the URL is "http://localhost:6969", then set this value to `6969`.

-`chatterClientId` [String]
This is the "Client ID" field of your chatter application in the [Twitch Console](https://dev.twitch.tv/console/apps).

-`chatterClientSecret` [String]
This is the "Client Secret" field of your chatter application in the [Twitch Console](https://dev.twitch.tv/console/apps).
You may have to click the "New Secret" button to get this value.
Note: If you click the "New Secret" button again, the previous secret will be invalid.

-`chatterScope` [List of Strings]
This is a list of [Twitch Scopes](https://dev.twitch.tv/docs/authentication/scopes/#twitch-api-scopes).
These scopes determine what the chatter application is allowed to do.

-`collectorClientId` [String]
This is the "Client ID" field of your collector application in the [Twitch Console](https://dev.twitch.tv/console/apps).

-`collectorClientSecret` [String]
This is the "Client Secret" field of your collector application in the [Twitch Console](https://dev.twitch.tv/console/apps).
You may have to click the "New Secret" button to get this value.
Note: If you click the "New Secret" button again, the previous secret will be invalid.

-`collectorScope` [List of Strings]
This is a list of [Twitch Scopes](https://dev.twitch.tv/docs/authentication/scopes/#twitch-api-scopes).
These scopes determine what the collector application is allowed to do.

-`socketKeepaliveBuffer` [Integer]
This is the number of milliseconds that the bot will wait after `socketKeepaliveTimeout` before assuming that it has disconnected.
Setting this value too low can result in false disconnect detections.

-`socketKeepaliveTimeout` [Integer]
This is the number of seconds (10-600) Twitch will wait before sending a [Keepalive message](https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message).
If Twitch has not sent a message in a while, it will send a keepalive message to ensure the connection is still healthy.

-`tokenExpirationBuffer` [Integer]
This is the number of milliseconds before the access token's expiration date that it will refresh the access token.
An access token is needed for some interactions with Twitch. These tokens do not last forever, and need to be refreshed.
If an interaction is attempted within `tokenExpirationBuffer` milliseconds of the token's expiration date, it will be refreshed before the interaction is processed.

### Authorize

When you run the bot for the first time, it should open a browser tab for you to authorize both the chatter and collector application.

**Log in to the bot's Twitch account when authorizing the chatter application.**

**Log in to the broadcaster's Twitch account when authorizing the collector application.**

There may be another dialog to redirect you to a confirmation page.

After you close the bot, it should save your authorization information in the `cache.json` file located in the project folder.
This makes it so that you do not need to authorize again.
If you do want to authorize again (for example, if you used the wrong Twitch account), make sure the bot is not running, then delete the `cache.json` file.
When you run the bot again, it should prompt you to authorize again.
If you did use the wrong Twitch account to authorize, make sure to disconnect the application in that account's [settings](https://www.twitch.tv/settings/connections).
