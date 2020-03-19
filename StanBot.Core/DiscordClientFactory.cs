namespace StanBot.Core
{
    using Discord.WebSocket;

    public class DiscordClientFactory
    {
        private readonly Communicator communicator;

        public DiscordClientFactory(Communicator communicator)
        {
            this.communicator = communicator;
        }

        public DiscordClient Create()
        {
            DiscordSocketClient discordSocketClient = new DiscordSocketClient();
            return new DiscordClient(discordSocketClient, this.communicator);
        }
    }
}