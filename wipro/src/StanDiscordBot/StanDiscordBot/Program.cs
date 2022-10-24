
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StanBot.Services;

namespace StanBot
{
    public class Program
    {

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _discordSocketClient;
        private IConfiguration _configuration;

        public async Task MainAsync()
        {

            _discordSocketClient = new DiscordSocketClient();
            _configuration = BuildConfiguration();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();

            await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration["DiscordApplicationToken"]);
            await _discordSocketClient.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_discordSocketClient)
                // add service
                // add logging
                .AddLogging()
                .AddSingleton<LogService>()
                // extra
                .AddSingleton(_configuration)
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile($"stan.json")
                .Build();
        }
    }
}