# Stonebot

A tool for making your own Twitch chat bot. Write Python scripts to have your bot interact with chat.

## Installation

- Download and extract the appropriate `exe` for your system from the releases tab.
- (Optional, but recommended) Download and extract the `lib` folder from the releases tab. The `lib` folder includes standard libraries for scripting.
- If you want to use the `lib` folder:
  - Run the `exe`. This will create a `Stonebot` directory in your local AppData folder (ex: `C:\Users\<user>\AppData\local\Stonebot`).
  - Move the `lib` folder into the `\scripts` directory in the `\Stonebot` folder (should look something like `C:\Users\<user>\AppData\local\Stonebot\scripts\lib`).

## Setup

- Create a Twitch application:
  - Navigate to https://dev.twitch.tv/console.
  - Click "Register Your Application".
  - Name your bot.
  - In the "OAuth Redirect URLs" field, put "http://localhost".
  - Select the Chat Bot category.
  - Select the Public client type.
  - Complete the captcha if applicable.
  - Click "Create".
  - Copy the Client ID for the next step.
- Configure Stonebot:
  - Run the `exe` if it's not already running.
  - Click on the Config button in the top right.
  - Enter the username of the broadcaster that you want to connect the bot to (not case sensitive).
  - Paste your client ID in the "Client ID" field.
  - Click "Save" in the top right.
  - Click "Authorize" at the top.
  - Follow the instructions in the popup (make sure you are logged in to the chatter's Twitch account when authorizing).
  - Click "Done" on the popup.

## Uninstallation

- Delete the `\Stonebot` directory in your local AppData folder (ex: `C:\Users\<user>\AppData\local\Stonebot`). WARNING: this will delete all scripts.
- Delete the `exe`.

## Usage

### General

Clicking the "Connect"/"Disconnect" button in the top left will connect/disconnect your bot to the broadcaster's chat.

Interactions (commands) can be enabled/disabled by clicking the power button next to their name in the dashboard. A green interaction is enabled, a red interaction is disabled.

Interactions can be edited by clicking the arrow icon under its name.

Clicking the "Edit Script" button under an interaction will open the Python script by your default configured program.

Clicking the trash can icon under an interaction will delete it.

### Commands

To create a new command, click the "New Command" button.
The command's name will be used by chatters with an ! prior to trigger it (ex: !youtube). The name must not be the same as other commands and must contain alphanumeric characters (aka: abc123).

The command's permission level determines who is allowed to use the command. Anyone with a permission level at or above the one set will be able to use the command. For example, a command with a permission level set to VIP can only be used by VIPs, moderators, and the broadcaster.

The command's cooldown determines how frequently it can be used. If a chatter tries to use the command before the cooldown, it will not trigger.

### Scripting

Stonebot includes Python 3.4, so it is not necessary to download Python, but it is recommended and can be found at https://www.python.org/downloads/.

Scripts are generated with some code to help with type checking + autocomplete features that your editor may have. This code is not necessary for your scripts to work.

## Contributing
