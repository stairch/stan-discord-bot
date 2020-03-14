namespace StanBot.Core
{
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class Orchestrator
    {
        private readonly DiscordSocketClient discordSocketClient;

        public Orchestrator(DiscordSocketClient discordSocketClient)
        {
            this.discordSocketClient = discordSocketClient;
        }

        public async Task LoginAsync()
        {
            await this.discordSocketClient.LoginAsync(TokenType.Bot, "Some Token");
            await this.discordSocketClient.StartAsync();
        }
    }
}