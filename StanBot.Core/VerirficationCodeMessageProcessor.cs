namespace StanBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class VerirficationCodeMessageProcessor : IMessageProcessor
    {
        private readonly VerificationCodeManager verificationCodeManager;

        public VerirficationCodeMessageProcessor(VerificationCodeManager verificationCodeManager)
        {
            this.verificationCodeManager = verificationCodeManager;
            this.ShouldContinueProcessing = false;
            this.MessageShouldTargetBot = true;
            this.AllowedMessageSources = new List<MessageSource> { MessageSource.User };
        }

        public bool ShouldContinueProcessing { get; }

        public bool MessageShouldTargetBot { get; }

        public IEnumerable<MessageSource> AllowedMessageSources { get; }

        public bool IsMatch(string message)
        {
            Regex regex = new Regex("^\\d{6}");
            return regex.IsMatch(message);
        }

        public async Task ProcessAsync(SocketMessage message)
        {
            int verificationCode = Convert.ToInt32(message.Content);
            SocketUser messageAuthor = message.Author;
            bool isCodeCorrect = this.verificationCodeManager.IsCodeCorrectForUser(verificationCode, messageAuthor.Id);

            if (isCodeCorrect)
            {
                SocketGuild socketGuild = messageAuthor.MutualGuilds.Single(sg => sg.CurrentUser != null && sg.CurrentUser.Guild.Id == sg.Id);
                SocketGuildUser socketGuildUser = socketGuild.Users.Single(sgu => sgu.Id == messageAuthor.Id);
                SocketRole socketRole = socketGuild.Roles.Single(sr => sr.Name == "@student");
                await socketGuildUser.AddRoleAsync(socketRole);
            }
        }
    }
}