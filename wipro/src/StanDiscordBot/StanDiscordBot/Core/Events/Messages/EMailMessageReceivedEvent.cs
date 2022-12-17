using Discord;
using Discord.WebSocket;
using NLog;
using StanBot.Services.ErrorNotificactionService;
using StanBot.Services.MailService;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;

namespace StanBot.Core.Events.Messages
{
    internal class EMailMessageReceivedEvent : IMessageReceiver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly DatabaseErrorNotificationService _errorNotificationService;

        private readonly IStudentRepository _studentRepository;
        private readonly VerificationCodeManager _verificationCodeManager;
        private readonly IMailService _mailService;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public EMailMessageReceivedEvent(
            IStudentRepository studentRepository, 
            VerificationCodeManager verificationCodeManager, 
            IMailService mailService,
            DatabaseErrorNotificationService errorNotificationService)
        {
            _errorNotificationService = errorNotificationService;
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
            catch (Exception exception)
            {
                // Send Mail to Admin, because of connection problems
                _errorNotificationService.SendDatabaseErrorToAdmins(exception, "EmailMessageReceivedEvent");
                _logger.Error($"There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {exception.Message}");

                await message.Channel.SendMessageAsync("Es gab einen Fehler bei der Abfrage deiner E-Mail. Ein Administrator wurde schon kontaktiert. " +
                    "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                    "There was an error retrieving your email. An administrator has already been contacted. Please be patient and try again later.");
                return;
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

            try
            {
                await _mailService.SendMailToAsync(message.Content, "STAIR Discord Verification", messageBody);

                await message.Channel.SendMessageAsync($"Vielen Dank! Ich habe ein Mail an {message.Content} geschickt.\n\rThanks! I've sent a mail to {message.Content}.");
                _logger.Info($"Successfully send Email to {message.Content} with Verification Code {verificationCode}");
            } 
            catch
            {
                await message.Channel.SendMessageAsync($"Es gab ein Fehler beim Senden der E-Mail. Der Administrator wurde schon kontaktiert. " +
                    "Habe ein wenig Geduld und versuche es später noch einmal.\n\r" +
                    "There was an error sending the email. The administrator has already been contacted. Have a little patience and try again later.");
            }
        }

        public bool IsMatch(SocketMessage message)
        {
            return StudentUtil.IsStudentEmailFormatValid(message.Content);
        }
    }
}
