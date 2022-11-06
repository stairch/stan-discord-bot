using Discord;
using Discord.WebSocket;
using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;
using System.Text.RegularExpressions;

namespace StanBot.Core.Events.Messages
{
    internal class EMailMessageReceivedEvent : IMessageReceiver
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IDiscordAccountRepository _discordAccountRepository;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public EMailMessageReceivedEvent(IStudentRepository studentRepository, IDiscordAccountRepository discordAccountRepository)
        {
            _studentRepository = studentRepository;
            _discordAccountRepository = discordAccountRepository;

            AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            ChannelType = typeof(SocketDMChannel);
        }

        public async Task ProcessMessage(SocketUserMessage message)
        {
            Console.WriteLine($"Authentication Message received: {message}");

            /*try
            {

                Student? student = _studentRepository.FindWithEmail(message.Content);
                if (student == null)
                {
                    await message.Channel.SendMessageAsync(
                        $"Ein Student mit der E-Mail Addresse {message.Content} existiert nicht."
                        + "Kontrollieren Sie auf mögliche Tippfehler oder kontaktieren Sie einen Administrator.\n\r\n\r"
                        + $"A student with the email address {message.Content} does not exist. Check for possible typos or contact an administrator."
                        );
                    return;
                }

                // TODO: generate Verification Code

                // Save new DiscordAccount linked to Student
                DiscordAccount discordAccount = new DiscordAccount(
                    message.Author.Username,
                    message.Author.DiscriminatorValue,
                    123456,
                    student.StudentId
                    );

                int accountId = _discordAccountRepository.Insert(discordAccount);
            }
            catch (LinqToDBException exception)
            {
                // Send Mail to Admin, because of connection problems
                Console.WriteLine(exception.Message);
            }*/

            // TODO: send Email to user.

            await message.Channel.SendMessageAsync($"Vielen Dank! Ich habe ein Mail an {message.Content} geschickt.\n\rThanks! I've sent a mail to {message.Content}.");
        }

        public bool IsMatch(SocketMessage message)
        {
            return StudentUtil.IsStudentEmailFormatValid(message.Content);
        }
    }
}
