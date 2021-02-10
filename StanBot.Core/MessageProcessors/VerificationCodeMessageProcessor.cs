namespace StanBot.Core.MessageProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class VerificationCodeMessageProcessor : IMessageProcessor
    {
        private readonly VerificationCodeManager verificationCodeManager;
        private readonly DiscordClient discordClient;

        public VerificationCodeMessageProcessor(VerificationCodeManager verificationCodeManager, DiscordClient discordClient)
        {
            this.verificationCodeManager = verificationCodeManager;
            this.discordClient = discordClient;
            this.ShouldContinueProcessing = false;
            this.MessageShouldTargetBot = true;
            this.AllowedMessageSources = new List<MessageSource> { MessageSource.User };
        }

        public bool ShouldContinueProcessing { get; }

        public bool MessageShouldTargetBot { get; }

        public IEnumerable<MessageSource> AllowedMessageSources { get; }

        public bool IsMatch(SocketMessage message)
        {
            Regex regex = new Regex("^\\d{6}");
            return regex.IsMatch(message.Content);
        }

        public async Task ProcessAsync(SocketMessage message)
        {
            int verificationCode = Convert.ToInt32(message.Content);
            SocketUser messageAuthor = message.Author;
            bool isCodeCorrect = this.verificationCodeManager.IsCodeCorrectForUser(verificationCode, messageAuthor.Id);

            if (isCodeCorrect == false)
            {
                NonBlockingLogger.Warn($"{messageAuthor.Username} provided a wrong verification code: {verificationCode}. Correct would have been: {this.verificationCodeManager.GetCodeForUser(messageAuthor.Id)}");
                await messageAuthor.SendMessageAsync(
                    "Hmmm... Es sieht aus, als wäre das der falsche Code. Bitte überprüfe, ob du mir den richtigen " +
                        "Code geschickt hast. Falls Ja, gib mir nochmals deine Mail Adresse, dann schicke ich dir ein neues Mail.\n\r" +
                        "Hmmm... It looks like that's the wrong code. Please make sure that you entered the code correctly. If you did, " +
                        "send me your mail address again and I'll send you another mail with a new verification code.");
                return;
            }

            SocketGuild socketGuild = messageAuthor.MutualGuilds.Single(sg => sg.CurrentUser != null && sg.CurrentUser.Guild.Id == sg.Id);
            SocketGuildUser socketGuildUser = socketGuild.Users.Single(sgu => sgu.Id == messageAuthor.Id);
            NonBlockingLogger.Info($"Verification code {verificationCode} is correct for user {messageAuthor.Username}");
            SocketRole socketRole = socketGuild.Roles.Single(sr => sr.Name == this.discordClient.StudentRoleName);
            await socketGuildUser.AddRoleAsync(socketRole);
            await socketGuildUser.SendMessageAsync("Danke vielmals. Du bist nun verifiziert als Student.\n\rThank you very much. You're now verified as a student.");
            NonBlockingLogger.Info($"Assigned role @student to {messageAuthor.Username}");
            this.verificationCodeManager.RemoveCodesForUser(messageAuthor.Id);
        }
    }
}