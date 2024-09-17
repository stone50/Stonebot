# Stone Bot
This is a chatbot for [Twitch](https://www.twitch.tv/).
 
## Setup

### Register Your Application

Follow [these steps](https://dev.twitch.tv/docs/authentication/register-app/) to register your application.

### Config

Add a `config.json` file to the project directory:
```
{
  "authorizationPort": 6969,
  "botClientId": <your application client id>,
  "botClientSecret": <your application client secret>,
  "broadcasterLogin": <your Twitch login>,
  "scope": [
    "user:bot",
    "user:read:chat",
    "user:write:chat"
  ],
  "socketKeepaliveBuffer": 5000,
  "socketKeepaliveTimeout": 10,
  "tokenExpirationBuffer": 1000
}
```
-`authorizationPort` [Integer]
This is the localhost port that will be used when you authenticate and allow the bot to interact with your Twitch channel.
This needs to be the same port that is specified in the "OAuth Redirect URLs" field in the [Twitch Console](https://dev.twitch.tv/console/apps).
For example, if the URL is "http://localhost:6969", then set this config value to `6969`.

-`botClientId` [String]
This is the "Client ID" field in the [Twitch Console](https://dev.twitch.tv/console/apps).

-`botClientSecret` [String]
This is the "Client Secret" field in the [Twitch Console](https://dev.twitch.tv/console/apps).
You may have to click the "New Secret" button to get this value.
Note: If you click the "New Secret" button again, the previous secret will be invalid.

-`broadcasterLogin` [String]
This is your Twitch channel username.

-`scope` [List of Strings]
This is a list of [Twitch Scopes](https://dev.twitch.tv/docs/authentication/scopes/#twitch-api-scopes).
These scopes determine what the bot is allowed to do.
Read the docs to find out which ones you need.
Note: The scopes in the example allow for most basic chatbot interactions, so you probably do not need to change them.

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
