namespace StanBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

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
            this.regex = new Regex("(\\w*.\\w*@stud.hslu.ch)");
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

            string messageBody = $"Hello {message.Author.Username}\n\r"
                                 + $"Here is your verification code for the STAIR discord server: {verificationCode}\n\r"
                                 + $"If you have any questions about me, the discord server or about STAIR please don't "
                                 + $"hesitate to ask a STAIR member (marked green on discord).\n\r\n\r"
                                 + $"Kind regards\n"
                                 + $"Stan";

            await this.mailService.SendMailToAsync(message.Content, "STAIR Discord Verification", messageBody);
        }
    }
}