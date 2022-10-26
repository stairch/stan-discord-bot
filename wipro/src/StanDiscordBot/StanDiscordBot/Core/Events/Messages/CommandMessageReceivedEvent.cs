using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanBot.Core.Events.Messages
{
    public class CommandMessageReceivedEvent : IMessageReceiver
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private IServiceProvider _serviceProvider;

        public CommandMessageReceivedEvent(DiscordSocketClient discordSocketClient, CommandService commandService, IServiceProvider serviceProvider)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }


        public async Task ProcessMessage(SocketMessage socketMessage)
        {
            // Ignore system messages and messages from bots
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!(message.HasStringPrefix(Config.Get().Prefix, ref argPos) || message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))) return;

            Console.WriteLine("Message with command prefix received.");
            var context = new SocketCommandContext(_discordSocketClient, message);
            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
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
