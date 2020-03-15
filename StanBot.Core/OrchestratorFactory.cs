namespace StanBot.Core
{
    using Discord.WebSocket;

    public class OrchestratorFactory
    {
        private readonly MailService mailService;

        private readonly VerificationCodeManager verificationCodeManager;

        public OrchestratorFactory(MailService mailService, VerificationCodeManager verificationCodeManager)
        {
            this.mailService = mailService;
            this.verificationCodeManager = verificationCodeManager;
        }

        public Orchestrator Create()
        {
            DiscordSocketClient discordSocketClient = new DiscordSocketClient();
            return new Orchestrator(discordSocketClient, this.mailService, this.verificationCodeManager);
        }
    }
}