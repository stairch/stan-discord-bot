using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;

namespace StanBot.Core.Events.Messages
{
    public class VerificationCodeMessageReceivedEvent : IMessageReceiver
    {
        private readonly Regex _regex;
        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public VerificationCodeMessageReceivedEvent()
        {
            _regex = new Regex("^\\d{6}");
            AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            ChannelType = typeof(SocketDMChannel);
        }
        public bool IsMatch(SocketMessage message)
        {
            return _regex.IsMatch(message.Content);
        }

        public async Task ProcessMessage(SocketUserMessage message)
        {
            Console.WriteLine($"Verification Code received: {message}");

            // TODO: save Discord Account to DB with correct Verification Code, Account and Dates


            await message.Channel.SendMessageAsync("Danke vielmals. Du bist nun verifiziert als Student.\n\rThank you very much. You're now verified as a student.");
        }
    }
}
