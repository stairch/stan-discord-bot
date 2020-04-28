namespace StanBot.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    using Ninject;

    using MessageProcessors;

    public class Communicator
    {
        private readonly IEnumerable<IMessageProcessor> messageReceivedProcessors;

        public Communicator([Named("MessageReceived")]IEnumerable<IMessageProcessor> messageReceivedProcessors)
        {
            this.messageReceivedProcessors = messageReceivedProcessors;
        }

        public async Task DiscordClientOnMessageReceived(SocketMessage message)
        {
            bool wasMessageProcessed = false;
            foreach (IMessageProcessor handle in this.messageReceivedProcessors)
            {
                if (this.IsMessageSourceCorrect(message, handle) && handle.IsMatch(message))
                {
                    await handle.ProcessAsync(message);
                    wasMessageProcessed = true;
                    if (handle.ShouldContinueProcessing == false)
                    {
                        return;
                    }
                }
            }

            if (message.Channel is IDMChannel && wasMessageProcessed == false)
            {
                await message.Author.SendMessageAsync("Entschuldigung, aber das habe ich nicht verstanden. Bitte überprüfe, dass du keinen Schreibfehler gemacht hast.\n\r" +
                                                      "Sorry but I didn't understand that. Please check that you didn't make any spelling errors.");
            }
        }

        private bool IsMessageSourceCorrect(IMessage message, IMessageProcessor handle)
        {
            return handle.AllowedMessageSources.Contains(message.Source)
                && (handle.MessageShouldTargetBot == false  || message.Channel.GetType() == typeof(SocketDMChannel));
        }
    }
}