namespace StanBot.Service
{
    using System.Threading.Tasks;

    using StanBot.Core;
    using StanBot.Core.MailService;
    using StanBot.Service.Configuration;

    // ReSharper disable once ClassNeverInstantiated.Global - Jusified because it uses dependency injection
    public class ServiceManager
    {
        private readonly Orchestrator orchestrator;

        private readonly ConfigLoader configLoader;

        private readonly IMailService mailService;

        public ServiceManager(Orchestrator orchestrator, ConfigLoader configLoader, IMailService mailService)
        {
            this.orchestrator = orchestrator;
            this.configLoader = configLoader;
            this.mailService = mailService;
        }

        public async Task Start()
        {
            Config config = this.configLoader.LoadConfigFromFile();
            NonBlockingLogger.Info($"Service started with the following config: {config}");
            await this.mailService.Initialize(config.FromMailAddress, config.FromName, config.AppId, config.Scopes);
            await this.orchestrator.InitializeDiscordAsync(config.DiscordApplicationToken, config.GuildName, config.AnnouncementRoleName, config.StudentRoleName);
        }

        public void Stop()
        {
            NonBlockingLogger.Info("Service stopped!");
        }
    }
}