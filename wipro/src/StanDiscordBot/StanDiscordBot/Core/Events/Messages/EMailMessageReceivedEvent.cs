using Discord;
using Discord.WebSocket;
using LinqToDB;
using NLog;
using StanBot.Services.MailService;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;

namespace StanBot.Core.Events.Messages
{
    internal class EMailMessageReceivedEvent : IMessageReceiver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IStudentRepository _studentRepository;
        private readonly VerificationCodeManager _verificationCodeManager;
        private readonly IMailService _mailService;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public EMailMessageReceivedEvent(IStudentRepository studentRepository, VerificationCodeManager verificationCodeManager, IMailService mailService)
        {
            _studentRepository = studentRepository;
            _verificationCodeManager = verificationCodeManager;
            _mailService = mailService;

            AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            ChannelType = typeof(SocketDMChannel);
        }

        public async Task ProcessMessage(SocketUserMessage message)
        {
            _logger.Debug($"Authentication Message received: {message} | From: {message.Author}");

            try
            {

                Student? student = _studentRepository.FindWithEmail(message.Content);
                if (student == null)
                {
                    await message.Channel.SendMessageAsync(
                        $"Ein Student mit der E-Mail Addresse {message.Content} existiert nicht. "
                        + "Kontrolliere auf mögliche Tippfehler oder kontaktiere einen Administrator.\n\r\n\r"
                        + $"A student with the email address {message.Content} does not exist. Check for possible typos or contact an administrator."
                        );
                    _logger.Info($"Could not find a student with Email: {message.Content}");
                    return;
                }
            } 
            catch (LinqToDBException exception)
            {
                // Send Mail to Admin, because of connection problems
                _logger.Error($"There was an Error, due to a database exception in the Email Message Received Event. Stacktrace: {exception.Message}");
            }

            int verificationCode = _verificationCodeManager.CreateCodeForUser(message.Author.Id, message.Content);

            string messageBody = $"Hi {message.Author.Username}\n\r"
                                 + $"Hier ist dein Verifizierungscode für den STAIR Discord Server: {verificationCode}.\n\r"
                                 + $"Bitte kopiere den Code und sende ihn mir persönlich via Discord. Danach bekommst du "
                                 + $"die @student Rolle, sowie dein Haus auf dem STAIR Discord Server zugewiesen.\n\r"
                                 + $"Falls du irgendwelche Fragen bezüglich mir, dem Discord Server oder STAIR hast, "
                                 + $"zögere nicht ein STAIR Mitglied zu fragen (grün markiert im Discord).\n\r\n\r"
                                 + $"Here is your verification code for the STAIR Discord server: {verificationCode}\n\r"
                                 + $"Please copy the verification code and send it to me via Discord. Then you'll be "
                                 + $"assigned the @student role, as well as your house on the STAIR Discord server\n\r"
                                 + $"If you have any questions about me, the Discord server or about STAIR please don't "
                                 + $"hesitate to ask a STAIR member (marked green on Discord).\n\r\n\r"
                                 + $"Liebe Grüsse/Kind regards\n"
                                 + $"Stan";

            await _mailService.SendMailToAsync(message.Content, "STAIR Discord Verification", messageBody);

            await message.Channel.SendMessageAsync($"Vielen Dank! Ich habe ein Mail an {message.Content} geschickt.\n\rThanks! I've sent a mail to {message.Content}.");
            _logger.Info($"Successfully send Email to {message.Content} with Verification Code {verificationCode}");
        }

        public bool IsMatch(SocketMessage message)
        {
            return StudentUtil.IsStudentEmailFormatValid(message.Content);
        }
    }
}
