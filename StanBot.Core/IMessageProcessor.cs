namespace StanBot.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Discord;

    public interface IMessageProcessor
    {
        bool ShouldContinueProcessing { get; }

        bool MessageShouldTargetBot { get; }

        IEnumerable<MessageSource> AllowedMessageSources { get; }

        bool IsMatch(string message);

        Task ProcessAsync(IMessage message);
    }
}