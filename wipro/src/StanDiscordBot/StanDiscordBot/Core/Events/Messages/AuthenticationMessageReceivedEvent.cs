using Discord.WebSocket;

namespace StanBot.Core.Events.Messages
{
    internal class AuthenticationMessageReceivedEvent : IMessageReceiver
    {
        public Task ProcessMessage(SocketMessage socketMessage)
        {
            Console.WriteLine($"Authentication Message received: {socketMessage}");
            return Task.CompletedTask;
        }
    }
}
