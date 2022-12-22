using Discord;
using Discord.WebSocket;

namespace StanBot.Core.Events.Messages
{
    public interface IMessageReceiver
    {
        IEnumerable<MessageSource> AllowedMessageSources { get; }

        Type ChannelType { get; }

        bool IsMatch(SocketMessage message);

        Task ProcessMessage(SocketUserMessage socketMessage);

    }
}
