using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using StanDatabase.Models;
using StanDatabase.Repositories;
using LinqToDB;

namespace StanBot.Core.Events.Messages
{
    public class VerificationCodeMessageReceivedEvent : IMessageReceiver
    {
        private readonly Regex _regex;
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IDiscordAccountDiscordRoleRepository _discordAccountDiscordRoleRepository;
        private readonly IDiscordRoleRepository _discordRoleRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly VerificationCodeManager _verificationCodeManager;

        public IEnumerable<MessageSource> AllowedMessageSources { get; }
        public Type ChannelType { get; }

        public VerificationCodeMessageReceivedEvent(
            VerificationCodeManager verificationCodeManager, 
            IDiscordAccountRepository discordAccountRepository, 
            IStudentRepository studentRepository,
            IDiscordAccountDiscordRoleRepository discordAccountDiscordRoleRepository,
            IDiscordRoleRepository discordRoleRepository,
            IHouseRepository houseRepository)
        {
            _regex = new Regex("^[0-9]{6}$");
            _discordAccountRepository = discordAccountRepository;
            _studentRepository = studentRepository;
            _verificationCodeManager = verificationCodeManager;
            _discordAccountDiscordRoleRepository = discordAccountDiscordRoleRepository;
            _discordRoleRepository = discordRoleRepository;
            _houseRepository = houseRepository;

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
            int verificationCode = Convert.ToInt32(message.Content);
            SocketUser messageAuthor = message.Author;
            bool isCodeCorrect = _verificationCodeManager.IsCodeCorrectForUser(verificationCode, messageAuthor.Id);

            if (isCodeCorrect == false)
            {
                await message.Channel.SendMessageAsync(
                    "Hmmm... Es sieht aus, als wäre das der falsche Code. Bitte überprüfe, ob du mir den richtigen " +
                    "Code geschickt hast. Falls Ja, gib mir nochmals deine Mail Adresse, dann schicke ich dir ein neues Mail.\n\r" +
                    "Hmmm... It looks like that's the wrong code. Please make sure that you entered the code correctly. If you did, " +
                    "send me your mail address again and I'll send you another mail with a new verification code.");
                return;
            }

            string email = _verificationCodeManager.getEmaiForUser(verificationCode, messageAuthor.Id);
            List<DiscordRole> roles = new List<DiscordRole>();
            try
            {
                Student? student = _studentRepository.FindWithEmail(email);
                if (string.IsNullOrEmpty(email) || student == null)
                {
                    await message.Channel.SendMessageAsync(
                        "Hmmm... Es sieht so aus als würde deine E-Mail, dein Discord Account und der Code nicht zusammenpassen. " +
                        "Bitte überprüfe ob du mit dem gleichen Discord Account eingeloggt bist, als du mir die E-Mail geschickt hast " +
                        "und wiederhole den Vorgang.\n\r" +
                        "Hmmm... It looks like your email, Discord account and code don't match. " +
                        "Please check if you are logged in with the same Discord account when you sent me the email and repeat the process..");
                    return;
                }

                // Check if User already exists in database
                if (_discordAccountRepository.DoesDiscordAccountExist(messageAuthor.DiscriminatorValue, messageAuthor.Username))
                {
                    // if account already exists, just get roles and assign them
                    DiscordAccount discordAccount = _discordAccountRepository.GetAccount(messageAuthor.DiscriminatorValue, messageAuthor.Username)!;
                    roles = _discordAccountDiscordRoleRepository.GetRolesForAccount(discordAccount.DiscordAccountId);
                    Console.WriteLine("Account already existed");
                } 
                else
                {
                    // if account does not exists, create it, link with roles and assign them
                    roles = CreateDiscordAccountAndLinkRoles(messageAuthor, student);
                    Console.WriteLine("Create new account");
                }
            }
            catch (LinqToDBException exception)
            {
                // Send Mail to Admin, because of database problems
                Console.WriteLine(exception.Message);

                await message.Channel.SendMessageAsync(
                    "So wie es aussieht, ist bei der Einschreibung von meiner Seite ein Fehler unterlaufen. " +
                    "Bitte kontaktiere einen Administrator.\n\r" +
                    "As it looks, there was a mistake on my side during the enrollment. Please contact an administrator."
                    );
                return;
            }

            // assign roles to discord user
            SocketGuild socketGuild = messageAuthor.MutualGuilds.Single(guild => guild.CurrentUser != null && guild.CurrentUser.Guild.Id == guild.Id);
            SocketGuildUser socketGuildUser = socketGuild.Users.Single(guildUser => guildUser.Id == messageAuthor.Id);
            foreach (DiscordRole role in roles)
            {
                SocketRole socketRole = socketGuild.Roles.Single(r => r.Name.Equals(role.DiscordRoleName));
                await socketGuildUser.AddRoleAsync(socketRole);
            }

            await message.Channel.SendMessageAsync("Danke vielmals. Du bist nun verifiziert als Student.\n\rThank you very much. You're now verified as a student.");
            _verificationCodeManager.RemoveCodeForUser(messageAuthor.Id);
        }

        private List<DiscordRole> CreateDiscordAccountAndLinkRoles(SocketUser author, Student student)
        {
            // Save new DiscordAccount linked to Student
            DiscordAccount discordAccount = DiscordAccount.CreateNew(
                author.Username,
                author.DiscriminatorValue,
                student
                );

            discordAccount.DiscordAccountId = _discordAccountRepository.Insert(discordAccount);

            // Get HouseRole from student
            DiscordRole houseRole = _houseRepository.getRoleForHouse(student.House);
            // get student role
            DiscordRole studentRole = _discordRoleRepository.GetRoleByName("student");

            DiscordAccountDiscordRole houseRoleLink = DiscordAccountDiscordRole.CreateNew(discordAccount, houseRole);
            DiscordAccountDiscordRole studentRoleLink = DiscordAccountDiscordRole.CreateNew(discordAccount, studentRole);

            _discordAccountDiscordRoleRepository.Insert(houseRoleLink);
            _discordAccountDiscordRoleRepository.Insert(studentRoleLink);

            return _discordAccountDiscordRoleRepository.GetRolesForAccount(discordAccount.DiscordAccountId);
        }
    }
}
