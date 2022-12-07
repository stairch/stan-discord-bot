using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using NLog;
using StanBot.Services;
using StanDatabase.DataAccessLayer;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanBot.Core.Commands
{
    public class UpdateStudentsCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly RoleService _roleService;

        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IDiscordAccountDiscordRoleRepository _discordAccountDiscordRoleRepository;
        private readonly IDiscordRoleRepository _discordRoleRepository;

        public UpdateStudentsCommand(
            IDiscordAccountRepository discordAccountRepository,
            IDiscordAccountDiscordRoleRepository discordAccountDiscordRoleRepository,
            IDiscordRoleRepository discordRoleRepository,
            RoleService roleService)
        {
            _discordAccountRepository = discordAccountRepository;
            _discordAccountDiscordRoleRepository = discordAccountDiscordRoleRepository;
            _discordRoleRepository = discordRoleRepository;
            _roleService = roleService;
        }

        [Command("updateStudents", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Updates the rights of the students. This possibly sets students to exstudent or student when they're students again")]
        // TODO: only show command to users that have the rights for it
        public async Task UpdateStudents([Remainder] string command = "")
        {
            _logger.Info($"UpdateStudents command received. Author: {Context.User} | Message: {command}");

            //string author = message.Author.Username;
            //if (discordAccountRepository.IsAdmin(author))
            if (true) // TODO
            {
                try
                {
                    IList<SocketGuildUser> currentStudentsOnDiscord = Context.Guild.Users.ToList();
                    IList<SocketGuildUser> notAuthenticatedUsers = new List<SocketGuildUser>();

                    foreach (SocketGuildUser socketGuildUser in currentStudentsOnDiscord)
                    {
                        if(_discordAccountRepository.DoesDiscordAccountExist(socketGuildUser.DiscriminatorValue, socketGuildUser.Username))
                        {
                            DiscordAccount discordAccount = _discordAccountRepository.GetAccount(socketGuildUser.DiscriminatorValue, socketGuildUser.Username)!;
                            DiscordRole studentRole = _discordRoleRepository.GetRoleByName("student");
                            DiscordRole exStudentRole = _discordRoleRepository.GetRoleByName("exstudent");

                            if(_discordAccountRepository.IsStillStudying(discordAccount))
                            {
                                // Change Role in Database from exstudent to student
                                _discordAccountDiscordRoleRepository.ChangeStillStudyingRole(discordAccount.AccountId, exStudentRole.DiscordRoleId, studentRole);

                                // Change Role in Discord
                                _roleService.AddRole(Context, socketGuildUser, "student");
                                _roleService.RemoveRole(Context, socketGuildUser, "exstudent");
                            } else
                            {
                                // Change Role in Database from student to exstudent
                                _discordAccountDiscordRoleRepository.ChangeStillStudyingRole(discordAccount.AccountId, studentRole.DiscordRoleId, exStudentRole);
                                DiscordRole houseRole = _discordAccountDiscordRoleRepository.GetHouseRoleForAccount(discordAccount.AccountId)!;
                                

                                // Change Role in Discord
                                _roleService.AddRole(Context, socketGuildUser, "exstudent");
                                _roleService.RemoveRole(Context, socketGuildUser, "student");
                                // Remove House Role in Discord
                                _roleService.RemoveRole(Context, socketGuildUser, houseRole.DiscordRoleName);
                            }
                        }
                        else
                        {
                            // Discord Account does not exists.
                            // Need to be registered to receive the student role
                            notAuthenticatedUsers.Add(socketGuildUser);
                        }
                    }
                    _logger.Info($"Successfully updated students and their rights.\nThey were {notAuthenticatedUsers.Count} users who are not authenticated and could not be updated!");
                    await ReplyAsync($"Loaded new students and updated their roles.\nThey were {notAuthenticatedUsers.Count} users who are not authenticated and could not be updated!");
                }
                catch (Exception ex)
                {
                    _logger.Error($"There was an error during updating students. Stacktrace: {ex.Message}");
                    await ReplyAsync("Loaded new users failed. Check the log for more information.");
                }
            }
            else
            {
                _logger.Warn($"Unauthorized user tries to update students! User: {Context.User.Username}");
                await ReplyAsync("You are not authorized to use this feature. Contact an admin if you think this is a mistake. Trying to hack this bot can lead to a ban of the STAIR Discord server!");
            }
        }
    }
}
