using Discord;
using Discord.WebSocket;

namespace StanBot.Core.Events.Messages
{
    public class MessageHandler
    {
        private readonly IEnumerable<IMessageReceiver> messageReceivers;

        public MessageHandler(IEnumerable<IMessageReceiver> messageReceivers)
        {
            this.messageReceivers = messageReceivers;
        }

        public async Task OnMessageReceived(SocketMessage socketMessage)
        {
            // Ignore System messages
            if (!(socketMessage is SocketUserMessage message)) return;

            foreach (IMessageReceiver receiver in messageReceivers)
            {
                if (IsMessageSourceCorrect(message, receiver) && receiver.IsMatch(message))
                {
                    Console.WriteLine("Sending message");
                    await receiver.ProcessMessage(message);
                }
            }
        }

        private bool IsMessageSourceCorrect(IMessage message, IMessageReceiver messageReceiver)
        {
            return messageReceiver.AllowedMessageSources.Contains(message.Source) && message.Channel.GetType() == messageReceiver.ChannelType;
        }
    }
}
