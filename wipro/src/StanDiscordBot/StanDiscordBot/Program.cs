
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StanBot.Services;

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
                    .AddSingleton<CommandManager>()
                    .AddLogging()
                    .AddSingleton<LogService>())
                .Build();

            await new Bot(host).RunAsync();
        }
    }
}