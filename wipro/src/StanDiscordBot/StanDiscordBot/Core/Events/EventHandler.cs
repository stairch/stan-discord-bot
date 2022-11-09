using Discord.Commands;
using Discord.WebSocket;
using StanBot.Core.Events.Messages;
using System.Reflection;

namespace StanBot.Core.Events
{
    public class EventHandler
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private readonly OnUserJoinedEvent _onUserJoinedEvent;
        private readonly MessageHandler _messageHandler;

        public EventHandler(
            DiscordSocketClient discordSocketClient,
            MessageHandler messageHandler,
            CommandService commandService,
            OnUserJoinedEvent onUserJoinedEvent)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _messageHandler = messageHandler;
            _onUserJoinedEvent = onUserJoinedEvent;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            Console.WriteLine("Initialize all Modules async. Then register DiscordSocketClient Events");
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);

            _discordSocketClient.UserJoined += _onUserJoinedEvent.OnUserJoined;
            _discordSocketClient.MessageReceived += _messageHandler.OnMessageReceived;
        }
    }
}
