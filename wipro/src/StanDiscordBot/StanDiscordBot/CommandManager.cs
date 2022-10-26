using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace StanBot
{
    public class CommandManager
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private IServiceProvider _serviceProvider;

        public CommandManager(DiscordSocketClient discordSocketClient, CommandService commandService, IServiceProvider serviceProvider)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _serviceProvider = serviceProvider;

            _discordSocketClient.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _serviceProvider = provider;
            Console.WriteLine("Command Manager initialized async");
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!(message.HasStringPrefix(Config.Get().Prefix, ref argPos) || message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))) return;

            Console.WriteLine("Message with command prefix received.");
            var context = new SocketCommandContext(_discordSocketClient, message);
            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                Console.Error.WriteLine(result.Error);
                var embed = new EmbedBuilder();
                embed.WithTitle("***Error***");

                if (result.ErrorReason == "The input text has too few parameters.")
                {
                    embed.WithDescription("This command requires something. Check help command to see what it needs");
                }
                else
                {
                    embed.WithDescription(result.ToString());

                }
                await context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }
    }
}
