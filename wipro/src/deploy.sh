# This script must be started on Linux or in WSL (on Windows)
echo "This script deploys the Stan Discord Bot to the server."
echo "Make sure that you're in the VPN or in Rotrkeuz on the campus."

# https://serverfault.com/questions/264595/can-scp-copy-directories-recursively
rsync -av ../../. localadmin@stair-bot-lnx.el.eee.intern:~/stan-discord-bot

rsync -av ../../../stair-config localadmin@stair-bot-lnx.el.eee.intern:~/.

ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stair-config/stanBot/stanBotConfig.json ~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"

ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stair-config/stanBot/stanDatabaseConfig.json ~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"

#ssh localadmin@stair-bot-lnx.el.eee.intern "cp ~/stan-discord-bot/wipro/src/StanDiscordBot/StanDiscordBot/service /etc/systemd/system/stanBot.service"

#ssh localadmin@stair-bot-lnx.el.eee.intern "sudo systemctl daemon-reload"
#ssh localadmin@stair-bot-lnx.el.eee.intern "sudo systemctl status stanBot"
