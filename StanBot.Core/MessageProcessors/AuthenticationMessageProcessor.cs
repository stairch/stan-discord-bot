namespace StanBot.Core.MessageProcessors
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    using StanBot.Core.MailService;

    public class AuthenticationMessageProcessor : IMessageProcessor
    {
        private readonly VerificationCodeManager verificationCodeManager;

        private readonly IMailService mailService;

        private readonly Regex regex;

        public AuthenticationMessageProcessor(VerificationCodeManager verificationCodeManager, IMailService mailService)
        {
            this.verificationCodeManager = verificationCodeManager;
            this.mailService = mailService;
            this.AllowedMessageSources = new List<MessageSource> { MessageSource.User };
            this.regex = new Regex("(\\S*@stud.hslu.ch)", RegexOptions.IgnoreCase);
        }

        public bool ShouldContinueProcessing { get; } = false;

        public bool MessageShouldTargetBot { get; } = true;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }

        public bool IsMatch(string message)
        {
            return this.regex.IsMatch(message);
        }

        public async Task ProcessAsync(SocketMessage message)
        {
            int verificationCode = this.verificationCodeManager.CreateCodeForUser(message.Author.Id);

            string messageBody = $"Hi {message.Author.Username}\n\r"
                                 + $"Hier ist dein Verifizierungscode für den STAIR Discord Server: {verificationCode}.\n\r"
                                 + $"Bitte kopiere den Code und sende ihn mir persönlich via Discord. Danach bekommst du "
                                 + $"die @student Rolle auf dem STAIR Discord Server zugewiesen.\n\r"
                                 + $"Falls du irgendwelche Fragen bezüglich mir, dem Discord Server oder STAIR hast, "
                                 + $"zögere nicht ein STAIR Mitglied zu fragen (grün markiert im Discord).\n\r\n\r"
                                 + $"Here is your verification code for the STAIR Discord server: {verificationCode}\n\r"
                                 + $"Please copy the verification code and send it to me via Discord. Then you'll be "
                                 + $"assigned the @student role on the STAIR Discord server\n\r"
                                 + $"If you have any questions about me, the Discord server or about STAIR please don't "
                                 + $"hesitate to ask a STAIR member (marked green on Discord).\n\r\n\r"
                                 + $"Liebe Grüsse/Kind regards\n"
                                 + $"Stan";

            await this.mailService.SendMailToAsync(message.Content, "STAIR Discord Verification", messageBody);
            await message.Channel.SendMessageAsync($"Vielen Dank! Ich habe ein Mail an {message.Content} geschickt.\n\rThanks! I've sent a mail to {message.Content}.");
            NonBlockingLogger.Info($"Sent verification mail to: {message.Author.Username} with mail: {message.Content}. Verification code: {verificationCode}");
        }
    }
}