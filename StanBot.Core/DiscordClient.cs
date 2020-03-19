namespace StanBot.Core
{
    using System.Text.RegularExpressions;
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
                "Hello good friend. I am Stan and I would like to welcome you to the STAIR discord.\n\r"
                + "Since this discord server is only for students at the HSLU department of computer"
                + " science I need to verify that you are enrolled there. Please enter your stud.hslu.ch"
                + " mail adress and I will send you a mail with the verfication instructions.");
        }
    }
}