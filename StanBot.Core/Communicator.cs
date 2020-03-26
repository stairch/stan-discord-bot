namespace StanBot.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    using Ninject;

    using StanBot.Core.MessageProcessors;

    public class Communicator
    {
        private readonly IEnumerable<IMessageProcessor> messageReceivedProcessors;

        public Communicator([Named("MessageReceived")]IEnumerable<IMessageProcessor> messageReceivedProcessors)
        {
            this.messageReceivedProcessors = messageReceivedProcessors;
        }

        public async Task DiscordClientOnMessageReceived(SocketMessage message)
        {
            foreach (IMessageProcessor handle in this.messageReceivedProcessors)
            {
                if (this.IsMessageSourceCorrect(message, handle) && handle.IsMatch(message.Content))
                {
                    await handle.ProcessAsync(message);
                    if (handle.ShouldContinueProcessing == false)
                    {
                        return;
                    }
                }
            }
        }

        private bool IsMessageSourceCorrect(IMessage message, IMessageProcessor handle)
        {
            return handle.AllowedMessageSources.Contains(message.Source)
                || (handle.MessageShouldTargetBot && message.Channel.GetType() == typeof(SocketDMChannel));
        }
    }
}