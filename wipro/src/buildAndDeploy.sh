# This script must be started on Linux or in WSL (on Windows)

# Note for Nicolas: convert to .ps1 script to run it with powershell

echo "This script deploys the Stan Discord Bot to the server."
echo "Make sure that you're in the VPN or in Rotrkeuz on the campus."
echo "Run this script in the folder its contained."

cd StanDiscordBot
dotnet publish -c Release -r linux-x64 --self-contained=false -p:PublishSingleFile=false -p:GenerateRuntimeConfigurationFiles=true -o artifacts
pwd
cd ..
pwd

# https://serverfault.com/questions/264595/can-scp-copy-directories-recursively
rsync -av ../../. localadmin@stair-bot-lnx.el.eee.intern:~/stan-discord-bot

rsync -av ../../../stair-config localadmin@stair-bot-lnx.el.eee.intern:~/.

ssh localadmin@stair-bot-lnx.el.eee.intern "mv /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/NLog.config /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"

ssh localadmin@stair-bot-lnx.el.eee.intern "touch ~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/discordAdmins.csv"

echo "In case of an update, run the following (don't do this when setting up the server for the first time):"
echo "$ ssh localadmin@stair-bot-lnx.el.eee.intern"
echo "$ sudo systemctl restart stanBot"
echo "$ sudo systemctl status stanBot"
echo "$ sudo journalctl -u stanBot -e # when there are problems"
