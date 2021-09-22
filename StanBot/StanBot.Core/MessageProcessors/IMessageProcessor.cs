using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StanBot.Core.MessageProcessors
{
    public interface IMessageProcessor
    {
        bool ShouldContinueProcessing { get; }

        bool MessageShouldTargetBot { get; }

        IEnumerable<MessageSource> AllowedMessageSources { get; }

        bool IsMatch(SocketMessage message);

        Task ProcessAsync(SocketMessage message);
    }
}