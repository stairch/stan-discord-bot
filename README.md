# Stan Discord Bot

This repository contains the STAIR discord bot named Stan. Also it contains some automation tools for setting up the Discord correctly.

## Getting started

* Copy stan.json to your StanBot.Service folder. You get the file from another developer or the virtual machine.
* Request access to the virtual machine.
* Request access to the Stan mailbox.
* Setup a C# dev IDE (Visual Studio 2019 recommended)

## Deployment

1. open package manager console in Visual Studio 1. Build:

        dotnet publish -c Release -r win-x64

2. Log in to VPN
3. Start "Remote Desktop Connection" in Windows and connect to `stair-bot.el.eee.intern`
4. Go to `C:/Services/StanBot` via file explorer.
5. Delete `StanBot.old` folder
6. Rename `StanBot` to `StanBot.old`
7. Create `StanBot` folder
8. Paste the new dlls from `StanBot.Service/bin/Release/netcoreapp3.1/win-x64` from your local build
9. Open "Services"
10. Right click and select "Restart" for the Discord Bot. Name: "STAIR Discord Bot"
11. Go into the log file and do what is written there\
    open <https://microsoft.com/devicelogin>\
    log in to your Microsoft STAIR account\
    enter code\
    accept
12. Go to chapter "check if bot is online and running"
13. Sign out

## Check if bot is online and running

You can check if it's online when you go to the Discord and check for the green point next to the Stan Bot.
If the point is not green, there is a problem.

You can also check if it works when you send the student email as a direct message.

## Workflow

When students join the Discord they don't have any rights.

They can get access to the channels when they authenticate themselfs with theyr student email from HSLU.

### Authentication

So, they send a message to the Stan-bot. He receives it. Then this bot here gets it over an API and sends a new random code to him.
The student then has to go to his email and send now the code to the bot over Discord.
The bot then gets the message again over the API and checks it. 
When it's correct, the student gets his role and with it the rights and access.

## Recover

Look in the `StanBot.log` file under `C:/Services/StanBot` for more information.

See the "Deployment" chapter on how to restart.

## Useful links

* <https://discord.gg/discord-api> (helper Discord for the used librarys)
* <https://support.discord.com/hc/en-us/community/posts/360056762431-Increase-channel-limit>
* <https://support.discord.com/hc/en-us/community/posts/360032363631-Increase-the-Max-Number-of-Roles-per-Server>

## Nadeko Bot

Nadeko is the bot that is used for students, so they can assign a module channel to their account.

### Most important commands

Sent this commands in a channel with Nadeko

| Command | Description |
| ------- | ----------- |
| .listperms | List permissions - which role can do what |
| .mp 2 3 | move permission 2 to position 3 |
| .rp 4 | remove permission that is at index 4 |
| .resetperms | reset permissions |
| .permrole stair | users require "stair" role in order to edit permissions |
| .arm disable student nsfw | 
| .aum enable stair | 
| .aliaslist | list all alias commands |
| .cr VRT | Create role |
| .setavatar www.link.com/image.png | edit the user image (only when selfhosting) |
| .asar VRT | Make it possible for a user to assign the role (here: "VRT") to himself/herself |
| .ctch testkanal3 | create channel "testkanal3" |
| .cmdmap "show KICKPROG" .iam KICKPROG | Add alias (alias is here: show KICKPROG, the command that gets executed in the background: .iam KICKPROG) |
| .stats | show statistics from bot |

### Links

* <https://nadeko.bot/commands>
* <https://top.gg/bot/116275390695079945>
* <https://scbot.readthedocs.io/en/stable/Commands%20List/>
* <https://epicbotnew.readthedocs.io/en/latest/Commands%20List/>
