# TODO delete this file
cd /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/ && dotnet publish -c Release -r linux-x64 --self-contained=true -p:PublishSingleFile=false -p:GenerateRuntimeConfigurationFiles=true -o artifacts
echo "Restart Stan bot"
sudo systemctl restart stanBot
echo "Get StanBot status"
sudo systemctl status stanBot
#sudo journalctl -u stanBot
