namespace StanBot.Core
{
    using Discord.WebSocket;

    public class OrchestratorFactory
    {
        private readonly MailService mailService;

        public OrchestratorFactory(MailService mailService)
        {
            this.mailService = mailService;
        }

        public Orchestrator Create()
        {
            DiscordSocketClient discordSocketClient = new DiscordSocketClient();
            return new Orchestrator(discordSocketClient, this.mailService);
        }
    }
}