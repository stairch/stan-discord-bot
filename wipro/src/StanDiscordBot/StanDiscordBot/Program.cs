﻿
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StanBot.Core.Events;
using StanBot.Core.Events.Messages;
using StanBot.Services;
using StanDatabase.DataAccessLayer;
using StanDatabase.Repositories;
using EventHandler = StanBot.Core.Events.EventHandler;

namespace StanBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Config.LoadConfig();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => services
                    .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                    {
                        GatewayIntents = GatewayIntents.All,
                        AlwaysDownloadUsers = true,
                        LogLevel = LogSeverity.Debug
                    }))
                    .AddSingleton(new CommandService(new CommandServiceConfig
                    {
                        DefaultRunMode = RunMode.Async,
                        LogLevel = LogSeverity.Debug
                    }))
                    .AddScoped<IStudentRepository, StudentRepository>()
                    .AddScoped<IDiscordAccountRepository, DiscordAccountRepository>()
                    .AddScoped<IDiscordAccountDiscordRoleRepository, DiscordAccountDiscordRoleRepository>()
                    .AddSingleton<EventHandler>()
                    .AddSingleton<MessageHandler>()
                    .AddSingleton<VerificationCodeManager>()
                    .AddScoped<IMessageReceiver, EMailMessageReceivedEvent>()
                    .AddScoped<IMessageReceiver, VerificationCodeMessageReceivedEvent>()
                    .AddScoped<IMessageReceiver, CommandMessageReceivedEvent>()
                    .AddScoped<OnUserJoinedEvent>()
                    .AddLogging()
                    .AddSingleton<LogService>())
                .Build();

            await new Bot(host).StartAsync();
        }
    }
}