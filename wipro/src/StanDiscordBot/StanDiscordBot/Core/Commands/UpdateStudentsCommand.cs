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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [Command("updateStudents", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Updates the rights of the students. This possibly sets students to exstudent or student when they're students again")]
        // TODO: only show command to users that have the rights for it
        public async Task UpdateStudents([Remainder] string command = "")
        {
            _logger.Info($"UpdateStudents command received. Author: {Context.User} | Message: {command}");

            IDiscordAccountRepository discordAccountRepository = new DiscordAccountRepository();
            //string author = message.Author.Username;
            //if (discordAccountRepository.IsAdmin(author))
            if (true) // TODO
            {
                try
                {
                    IStudentRepository studentRepository = new StudentRepository();
                    HouseRepository houseRepository = new HouseRepository();
                    List<Student> currentStudents = studentRepository.GetCurrentStudents();
                    IList<SocketGuildUser> currentStudentsOnDiscord = Context.Guild.Users.ToList();
                    RoleService roleService = new RoleService();

                    foreach (SocketGuildUser socketGuildUser in currentStudentsOnDiscord)
                    {
                        _logger.Debug($"Successfully updated students and their rights.");
                        if (discordAccountRepository.IsStillStudying(socketGuildUser.Username))
                        {
                            // TODO: make role names configurable
                            roleService.AddRole(Context, socketGuildUser, "student");
                            roleService.RemoveRole(Context, socketGuildUser, "exstudent");
                        }
                        else if (discordAccountRepository.IsExstudent(socketGuildUser.Username))
                        {
                            roleService.AddRole(Context, socketGuildUser, "exstudent");
                            roleService.RemoveRole(Context, socketGuildUser, "student");
                        }
                        foreach (String houseRoleName in houseRepository.GetHouseDiscordRoles())
                        {
                            if (discordAccountRepository.GetHouseFromStudent(socketGuildUser.Username).DiscordRole.DiscordRoleName.Equals(houseRoleName))
                            {
                                roleService.AddRole(Context, socketGuildUser, houseRoleName);
                            }
                            else
                            {
                                roleService.RemoveRole(Context, socketGuildUser, houseRoleName);
                            }
                        }
                    }
                    _logger.Info($"Successfully updated students and their rights.");
                    await ReplyAsync("Loaded new students and updated their roles.");
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
