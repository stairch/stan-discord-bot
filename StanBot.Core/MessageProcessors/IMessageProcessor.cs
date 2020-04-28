namespace StanBot.Core.MessageProcessors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public interface IMessageProcessor
    {
        bool ShouldContinueProcessing { get; }

        bool MessageShouldTargetBot { get; }

        IEnumerable<MessageSource> AllowedMessageSources { get; }

        bool IsMatch(SocketMessage message);

        Task ProcessAsync(SocketMessage message);
    }
}