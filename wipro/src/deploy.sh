# This script must be started on Linux or in WSL (on Windows)
echo "This script deploys the Stan Discord Bot to the server."
echo "Make sure that you're in the VPN or in Rotrkeuz on the campus."

cd StanDiscordBot
dotnet publish -c Release -r linux-x64 --self-contained=true -p:PublishSingleFile=true -p:GenerateRuntimeConfigurationFiles=true -o artifacts
pwd
cd ..
pwd

# https://serverfault.com/questions/264595/can-scp-copy-directories-recursively
rsync -av ../../. localadmin@stair-bot-lnx.el.eee.intern:~/stan-discord-bot

rsync -av ../../../stair-config localadmin@stair-bot-lnx.el.eee.intern:~/.

ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stair-config/stanBot/stanBotConfig.json ~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"

ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stair-config/stanBot/stanDatabaseConfig.json ~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"

#ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stan-discord-bot/wipro/src/StanDiscordBot/StanDiscordBot/service /etc/systemd/system/stanBot.service"

#ssh localadmin@stair-bot-lnx.el.eee.intern "sudo systemctl daemon-reload"
#ssh localadmin@stair-bot-lnx.el.eee.intern "sudo systemctl status stanBot"

echo "now run the following:"
echo "$ ssh localadmin@stair-bot-lnx.el.eee.intern"
echo "$ sudo systemctl restart stanBot"
echo "$ sudo systemctl status stanBot"
echo "$ sudo journalctl -u stanBot -e # when there are problems"
