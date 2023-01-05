# This script is the powershell version to deploy the discord bot to the linux server
# It requires the package Posh-SSH
# To install it run the following command in a powershell terminal: 
# Install-Module -Name Posh-SSH

echo "This script deploys the Stan Discord Bot to the server."
echo "Make sure that you're in the VPN or in Rotrkeuz on the campus."
echo "Run this script in the folder its contained."

cd StanDiscordBot
dotnet publish -c Release -r linux-x64 --self-contained=false -p:PublishSingleFile=false -p:GenerateRuntimeConfigurationFiles=true -o artifacts
pwd
cd ..
pwd

$Password = "XXX-XXX-XXX"
$User = "localadmin"
$ComputerName = "stair-bot-lnx.el.eee.intern"

$secpasswd = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential ($User, $secpasswd)

# Copy artifacts and NLog.config
$artifactsPath = (resolve-path .\StanDiscordBot\artifacts)
$configPath = (resolve-path .\StanDiscordBot\NLog.config)
Set-SCPItem -ComputerName $ComputerName -Credential $Credentials -Path $artifactsPath -Destination "~/stan-discord-bot/wipro/src/StanDiscordBot" -Verbose
Set-SCPItem -ComputerName $ComputerName -Credential $Credentials -Path $configPath -Destination "~/stan-discord-bot/wipro/src/StanDiscordBot/artifacts" -Verbose

# Invoke commands
$SessionID = New-SSHSession -ComputerName $ComputerName -Credential $Credentials
Invoke-SSHCommand -SSHSession $SessionID -Command "touch /home/localadmin/data/discordAdmins.csv"
Invoke-SSHCommand -SSHSession $SessionID -Command "chmod +x /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/StanBot.dll"
Invoke-SSHCommand -SSHSession $SessionID -Command "chmod +x /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/StanScripts.dll"
Remove-SSHSession $SessionID

# scp -rp (resolve-path ..\..\.) localadmin@stair-bot-lnx.el.eee.intern:~/stan-discord-bot
# scp -rp (resolve-path ..\..\..\stair-config\) localadmin@stair-bot-lnx.el.eee.intern:~/.
# ssh localadmin@stair-bot-lnx.el.eee.intern "mv /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/NLog.config /home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/"
# scp ../../../system.drawing.common/lib/net6.0/System.Drawing.Common.dll localadmin@stair-bot-lnx.el.eee.intern:/home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/

echo "In case of an update, run the following (don't do this when setting up the server for the first time):"
echo "$ ssh localadmin@stair-bot-lnx.el.eee.intern"
echo "$ sudo systemctl restart stanBot"
echo "$ sudo systemctl status stanBot"
echo "$ sudo journalctl -u stanBot -e # when there are problems"

# how to get dll version number
#monodis --assembly file.exe | grep Version
