using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace StanBot.Core.Events.Messages
{
    internal class EMailMessageReceivedEvent : IMessageReceiver
    {
        private readonly Regex _regex;
        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public EMailMessageReceivedEvent()
        {
            _regex = new Regex("(\\S*@stud.hslu.ch)", RegexOptions.IgnoreCase);
            AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            ChannelType = typeof(SocketDMChannel);
        }
        public bool IsMatch(SocketMessage message)
        {
            return _regex.IsMatch(message.Content);
        }

        public async Task ProcessMessage(SocketUserMessage message)
        {
            Console.WriteLine($"Authentication Message received: {message}");

            // TODO: send Email to user.
            await message.Channel.SendMessageAsync($"Vielen Dank! Ich habe ein Mail an {message.Content} geschickt.\n\rThanks! I've sent a mail to {message.Content}.");
        }

    }
}
