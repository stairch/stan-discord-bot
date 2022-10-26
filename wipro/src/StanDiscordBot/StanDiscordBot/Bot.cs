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

        public async Task RunAsync()
        {
            using IServiceScope serviceScope = _hostEnvironment.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            _discordSocketClient = provider.GetRequiredService<DiscordSocketClient>();

            provider.GetRequiredService<LogService>();
            await provider.GetRequiredService<CommandManager>().InitializeAsync(provider);

            await _discordSocketClient.LoginAsync(TokenType.Bot, Config.Get().DiscordApplicationToken);
            await _discordSocketClient.StartAsync();

            _discordSocketClient.Ready += ReadyEvent;

            await Task.Delay(-1);
        }

        private async Task ReadyEvent()
        {
            Console.WriteLine($"Ready Event triggered! {_discordSocketClient.CurrentUser.Username}");
            await _discordSocketClient.SetGameAsync($"{Config.Get().Prefix}help");
            await _discordSocketClient.SetStatusAsync(UserStatus.Online);
        }
    }
}
