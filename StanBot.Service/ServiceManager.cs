namespace StanBot.Service
{
    using System.Threading.Tasks;

    using StanBot.Core;

    // ReSharper disable once ClassNeverInstantiated.Global - Jusified because it uses dependency injection
    public class ServiceManager
    {
        private readonly OrchestratorFactory orchestratorFactory;

        private readonly ConfigLoader configLoader;

        private readonly MailService mailService;

        private Orchestrator orchestrator;

        public ServiceManager(OrchestratorFactory orchestratorFactory, ConfigLoader configLoader, MailService mailService)
        {
            this.orchestratorFactory = orchestratorFactory;
            this.configLoader = configLoader;
            this.mailService = mailService;
        }

        public async Task Start()
        {
            Config config = this.configLoader.LoadConfigFromFile();
            this.mailService.Initialize(config.FromMailAdress, config.SmtpServer, config.SmtpPort, config.SmtpUsername, config.SmtpPassword);
            this.orchestrator = this.orchestratorFactory.Create();
            await this.orchestrator.LoginAsync(config.DiscordApplicationToken);
            this.orchestrator.RegisterNewUserListener();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}