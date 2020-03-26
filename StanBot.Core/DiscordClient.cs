﻿namespace StanBot.Core
{
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class DiscordClient
    {
        private readonly BaseSocketClient discordClient;

        private readonly Communicator communicator;

        public DiscordClient(BaseSocketClient discordClient, Communicator communicator)
        {
            this.discordClient = discordClient;
            this.communicator = communicator;
        }

        public async Task LoginAsync(string token)
        {
            await this.discordClient.LoginAsync(TokenType.Bot, token);
            await this.discordClient.StartAsync();
        }

        public void RegisterListeners()
        {
            this.discordClient.UserJoined += this.DiscordSocketClientOnUserJoined;
            this.discordClient.MessageReceived += this.communicator.DiscordClientOnMessageReceived;
        }

        private async Task DiscordSocketClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            await socketGuildUser.SendMessageAsync(
                "Hello good friend. \n\r"
                + "I am Stan and I would like to welcome you to the STAIR discord.\n"
                + "Since this discord server is only for students at the HSLU department of computer"
                + " science I need to verify that you are enrolled there. Please enter your stud.hslu.ch"
                + " mail adress and I will send you a mail with the verfication instructions. Furthermore,"
                + " you'll accept the following rules by staying on the server and verifying your identity.\n\r"
                + "**Regeln** – ja, leider\n\r" 
                + "\t1) Anstandsregeln gelten auch online, und mit dem Beitreten auf dem Server bestätigt ihr, dass ihr sie kennt und anwendet\n"
                + "\t2) Gesunder Menschenverstand wird vorausgesetzt\n\r"
                + "\tWir gehen davon aus, dass dies ausreichend ist.\n"
                + "\tSollten die Regeln angepasst werden müssen, wird darüber im Channel #stair-announcements informiert – mit dem Verbleib auf dem Server gelten diese als akzeptiert.\n\r"
                + "**Rules** – shame, we know\n\r"
                + "\t1) Decency rules apply online as well. By entering this server, you confirm that you not only know but also comply with these rules\n"
                + "\t2) Commond sense is assumed\n\r"
                + "\tWe assume that the rules above suffice.\n"
                + "\tShould it be necessary to adapt the rules, you accept these by remaining on the server. Such a change will be published in #stair-announcements.");
        }
    }
}