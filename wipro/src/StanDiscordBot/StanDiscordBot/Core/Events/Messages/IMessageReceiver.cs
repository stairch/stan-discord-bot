using Discord.WebSocket;

namespace StanBot.Core.Events.Messages
{
    public interface IMessageReceiver
    {
        Task ProcessMessage(SocketMessage socketMessage);
    }
}
