namespace StanBot.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class Communicator
    {
        private readonly IEnumerable<IMessageProcessor> handles;

        public Communicator(IMessageProcessor[] messageProcessors)
        {
            this.handles = messageProcessors;
        }

        public async Task DiscordClientOnMessageReceived(IMessage message)
        {
            foreach (IMessageProcessor handle in this.handles)
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