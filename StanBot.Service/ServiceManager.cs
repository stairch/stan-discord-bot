namespace StanBot.Service
{
    using System;
    using System.Threading.Tasks;

    using StanBot.Core;

    // ReSharper disable once ClassNeverInstantiated.Global - Jusified because it uses dependency injection
    public class ServiceManager
    {
        private readonly DiscordClientFactory discordClientFactory;

        private readonly ConfigLoader configLoader;

        private readonly IMailService mailService;

        private DiscordClient discordClient;

        public ServiceManager(DiscordClientFactory discordClientFactory, ConfigLoader configLoader, IMailService mailService)
        {
            this.discordClientFactory = discordClientFactory;
            this.configLoader = configLoader;
            this.mailService = mailService;
        }

        public async Task Start()
        {
            Config config = this.configLoader.LoadConfigFromFile();
            await this.mailService.Initialize(config.FromMailAdress, config.AppId, config.Scopes);
            this.discordClient = this.discordClientFactory.Create();
            await this.discordClient.LoginAsync(config.DiscordApplicationToken);
            this.discordClient.RegisterListeners();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping the service.");
        }
    }
}