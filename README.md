# The Isle EVRIMA Player Tracker
This is a tool that displays a live player count using a Discord bot's status for a The Isle Evrima server using RCON.

Licensed under MIT License.

## Showcase
![Showcase](/docs/end_result.png)

# Pre-requisites
This project is written in C# using .NET 8 and requires the .NET 8 runtime.

 - .NET 8 Desktop Runtime (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
 - Enabled RCON on your The Isle Evrima server
 - A Discord bot and its token

# Getting started
1. First install the .NET 8 Desktop Runtime if you don't already have it.
2. Download the latest release (or build the project yourself) and copy the application files wherever you want.
3. Run the **TheIsleEvrimaPlayerTracker** executable for the first time, a default **config.ini** file will be created in the same directory.
4. Make sure your Evrima server has RCON set up.
5. Configure the application's **config.ini** file. (see the [Configuration](#configuration) section for details)
6. Run the executable again. If configured correctly, the application will run and start displaying the online players.

# Enabling RCON on Evrima server
To enable RCON on your The Isle Evrima server, open the `Game.ini` config file located in `...\Saved\Config\WindowsServer` and do the following.

Under `[/script/theisle.tigamesession]` add/edit these settings:
```ini
bRconEnabled=true
RconPassword=enter_rcon_password
RconPort=8888
```

And under `[/Script/Engine.Game]` add/edit these settings:
```ini
RconEnabled=true
RconPassword=enter_rcon_password
RconPort=8888
```

**NOTE**: Make sure the password and port are the same in both settings!

Also make sure to open port `8888` (or the port you set in the settings) in **Firewall**!

# Creating a Discord bot
To create a Discord bot, obtaining its token and adding it to your Discord server, use [this guide from Discord itself](https://discord.com/developers/docs/getting-started#step-1-creating-an-app).

# Configuration
Before running, the application needs to be configured and given the correct connection details. There are two ways to configure the application.

## INI config file
After running the application for the first time, a default **config.ini** file will be created, which looks like this:
```ini
[Tracker]
Interval=30000
MaxServerPlayers=0
DisplayPattern={online} / {max}

[Rcon]
Host=127.0.0.1
Port=8888
Password=your_rcon_password_here
Timeout=5000

[Discord]
BotToken=your_bot_token_here
```

### Sections
| Section | Description |
| --- | --- |
| Tracker | Contains values that configure the application itself. |
| Rcon | Contains values that configure the RCON connection to The Isle Evrima server. |
| Discord | Contains the values that configure the Discord bot connection. |

### Values
| Value | Description | Default value |
| --- | --- | --- |
| Tracker.**Interval** | Specifies the interval in milliseconds between each update. Each update means a query to the server's RCON. (Minimum: 10000) | `30000` |
| Tracker.**MaxServerPlayers** | The maximum amount of players that the Evrima server has configured. (only used for displaying) | `0` |
| Tracker.**DisplayPattern** | This will be the output to the Discord bot's activity status. Special keys between {} parantheses will be replaced with actual values. (see [Display pattern](#display-pattern) section for details) | `{online} / {max}` |
| Rcon.**Host** | The IP address of the server. Can be left at default value if the application will be running on the same machine as the Evrima server. | `127.0.0.1` |
| Rcon.**Port** | The port that the Evrima server has RCON running on. | `8888` |
| Rcon.**Password** | The password that is set for access to the Evrima RCON. | `-` |
| Rcon.**Timeout** | Connection timeout in milliseconds to the RCON server. | `5000` |
| Discord.**BotToken** | Token for the Discord bot that will be displaying the status. | `-` |

## Command line
You can also configure the application using command line arguments.

**Any specified command line configuration arguments will override the values set in the INI config file!**

To configure a single value using a command line argument, simply specify an argument like this:

`--Section.Value=your_desired_value`

For example, to configure the Rcon Host value, use this argument:

`--Rcon.Host=127.0.0.1`

Here is a full example of running the application using a command line (PowerShell):

`.\TheIsleEvrimaPlayerTracker.exe --Rcon.Host=127.0.0.1 --Tracker.Interval=15000`

**Values that are not specified in the arguments will be set to the ones from the INI config file.**

# Display pattern
You can the specify display pattern that will be written to the Discord bot's status. The default display pattern is `{online} / {max}`, which will translate to `0 / 100` if there are 0 players on the server and the `Tracker.MaxServerPlayers` value is set to 100.

## Keywords
These are special keywords that will be replaced in the display pattern with actual values.
| Keyword | Actual value |
| --- | --- |
| `{online}` | Replaced with the amount of players online on the server. |
| `{max}` | Replaced with the maximum player cap on the server. (as per the `Tracker.MaxServerPlayers` value in the config) |

## Other examples
You can also omit any keywords from your display pattern, so for example if you set the display pattern to `{online} players`, the bot's status will be set to `0 players` if there are 0 players on the server.