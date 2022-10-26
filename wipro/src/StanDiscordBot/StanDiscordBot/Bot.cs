using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using StanBot.Services;
using Microsoft.Extensions.Hosting;

namespace StanBot
{
    public class Bot
    {
        private DiscordSocketClient _discordSocketClient;
        private IHost _hostEnvironment;

        public Bot(IHost hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task StartAsync()
        {
            using IServiceScope serviceScope = _hostEnvironment.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            _discordSocketClient = provider.GetRequiredService<DiscordSocketClient>();

            provider.GetRequiredService<LogService>();
            await provider.GetRequiredService<EventHandler>().InitializeAsync(provider);

            if (string.IsNullOrWhiteSpace(Config.Get().DiscordApplicationToken)) return;
            await _discordSocketClient.LoginAsync(TokenType.Bot, Config.Get().DiscordApplicationToken);
            await _discordSocketClient.StartAsync();

            await Task.Delay(-1);
        }
    }
}
