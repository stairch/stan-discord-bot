using Discord;
using Discord.WebSocket;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanBot.Services.ErrorNotificactionService
{
    public class MailErrorNotificationService
    {
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly DiscordSocketClient _discordSocketClient;

        public MailErrorNotificationService(
            IDiscordAccountRepository discordAccountRepository,
            DiscordSocketClient discordSocketClient)
        {
            _discordAccountRepository = discordAccountRepository;
            _discordSocketClient = discordSocketClient;
        }

        /// <summary>
        /// Sends a discord direct message to all admins, with describtion of the mail service error
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="sourceClassName"></param>
        public async void SendMailErrorToAdmins(Exception exception, string sourceClassName)
        {
            IList<DiscordAccount> discordAdmins = _discordAccountRepository.GetAllDiscordAdminAccounts();
            IList<SocketGuildUser> currentStudentsOnDiscord = _discordSocketClient.GetGuild(StanBotConfigLoader.Config.GuildId).Users.ToList();

            var embed = new EmbedBuilder()
            {
                Color = Color.Red
            };
            embed.WithTitle("*** Mail Service Error ***");
            embed.AddField(field =>
            {
                field.Name = "Stacktrace";
                field.Value = exception.StackTrace;
            });
            embed.AddField(field =>
            {
                field.Name = "Error Message";
                field.Value = exception.Message;
            });
            embed.AddField(field =>
            {
                field.Name = "Source Class";
                field.Value = sourceClassName;
            });

            foreach (DiscordAccount admin in discordAdmins)
            {
                SocketGuildUser? adminUser = currentStudentsOnDiscord
                    .FirstOrDefault(students => students.Username == admin.Username && students.DiscriminatorValue == admin.AccountId);

                if (adminUser != null)
                {
                    await adminUser.SendMessageAsync(embed: embed.Build());
                }
            }
        }
    }
}
