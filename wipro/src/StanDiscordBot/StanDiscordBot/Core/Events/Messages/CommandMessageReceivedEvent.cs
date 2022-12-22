using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using NLog;

namespace StanBot.Core.Events.Messages
{
    public class CommandMessageReceivedEvent : IMessageReceiver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private IServiceProvider _serviceProvider;
        private readonly Regex _regex;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public CommandMessageReceivedEvent(DiscordSocketClient discordSocketClient, CommandService commandService, IServiceProvider serviceProvider)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _regex = new Regex(StanBotConfigLoader.Config.Prefix + "(\\S*)", RegexOptions.IgnoreCase);

            AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            ChannelType = typeof(SocketTextChannel);
        }

        public bool IsMatch(SocketMessage message)
        {
            return _regex.IsMatch(message.Content);
        }

        public async Task ProcessMessage(SocketUserMessage message)
        {
            int argPos = 0;
            if (!(message.HasStringPrefix(StanBotConfigLoader.Config.Prefix, ref argPos) || message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))) return;

            _logger.Debug("Message with command prefix received");
            var context = new SocketCommandContext(_discordSocketClient, message);
            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if (!result.IsSuccess || result.Error == CommandError.UnknownCommand)
            {
                _logger.Warn($"The command send, could not be processed. Error: {result.Error}");
                var embed = new EmbedBuilder();

                if (result.ErrorReason == "The input text has too few parameters.")
                {
                    embed.WithDescription("This command requires something. Check help command to see what it needs.");
                }
                else if (result.ErrorReason == "Unknown command.")
                {
                    embed.WithDescription($"This command is not available. Check the help command to see all commands.");
                }
                else
                {
                    embed.WithTitle("***Error***");
                    embed.WithDescription(result.ToString());

                }
                await context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }
    }
}
