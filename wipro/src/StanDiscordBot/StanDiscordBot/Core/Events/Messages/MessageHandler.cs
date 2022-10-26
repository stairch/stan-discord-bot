using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine($"Message received. Send to all message processors: {messageReceivers.Count()}");
            foreach (IMessageReceiver receiver in messageReceivers)
            {
                await receiver.ProcessMessage(socketMessage);
            }
        }
    }
}
