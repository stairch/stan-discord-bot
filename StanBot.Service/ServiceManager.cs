﻿namespace StanBot.Service
{
    using System.Threading.Tasks;

    using StanBot.Core;
    using StanBot.Core.MailService;
    using StanBot.Service.Configuration;

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
            NonBlockingLogger.Info($"Service started with the following config: {config}");
            await this.mailService.Initialize(config.FromMailAddress, config.FromName, config.AppId, config.Scopes);
            this.discordClient = this.discordClientFactory.Create();
            await this.discordClient.LoginAsync(config.DiscordApplicationToken);
            this.discordClient.RegisterListeners();
        }

        public void Stop()
        {
            NonBlockingLogger.Info("Service stopped!");
        }
    }
}