using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace StanBot.Core.MessageProcessors
{
    public interface IMessageProcessor
    {
        bool ShouldContinueProcessing { get; }

        bool MessageShouldTargetBot { get; }

        IEnumerable<MessageSource> AllowedMessageSources { get; }

        bool IsMatch(string message);

        Task ProcessAsync(SocketMessage message);
    }
}