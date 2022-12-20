using Discord;
using Discord.Commands;
using Google.Protobuf.WellKnownTypes;
using NLog;
using ScottPlot.Palettes;
using StanBot.Services.ErrorNotificactionService;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanBot.Core.Commands
{
    public class AddAdminCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IStudentRepository _studentRepository;
        private readonly DatabaseErrorNotificationService _databaseErrorNotificationService;

        AddAdminCommand(IStudentRepository studentRepository, DatabaseErrorNotificationService databaseErrorNotificationService)
        {
            _studentRepository = studentRepository;
            _databaseErrorNotificationService = databaseErrorNotificationService;
        }

        [Command("addAdmin")]
        [Alias("latency")]
        [Summary("Adds an admin account to the system, who receives emails if an error was detected")]
        public async Task AddAdmin(string email = "")
        {
            try
            {
                Student? student = _studentRepository.FindWithEmail(email);
                if (student != null)
                {
                    _studentRepository.SetStudentIsAdmin(student, true);
                    _databaseErrorNotificationService.AddAdminMail(email);

                    await ReplyAsync($"Der Student mit der Email {student.StudentEmail} wurde erfolglreich zum Discord Admin befördert.\n\r" +
                        $"Student with email {student.StudentEmail} was succesfully set as DiscordAdmin");
                    _logger.Info($"Student with email {student.StudentEmail} was succesfully set as DiscordAdmin");
                }
                else
                {
                    await ReplyAsync($"Der Student mit der Email {email} konnte nicht zum DiscordAdmin befördert werden. Der Student existiert nicht.\n\r " +
                        $"Could not set student with email {email} as DiscordAdmin. This student doesn't exists.");
                    _logger.Info($"Could not set student with email {email} as DiscordAdmin");
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"There was an Error, due to a database exception. Stacktrace: {ex.Message}");
                var embed = new EmbedBuilder()
                {
                    Color = Color.Red
                };
                embed.WithTitle("*** Mail Service Error ***");
                embed.AddField(field =>
                {
                    field.Name = "Stacktrace";
                    field.Value = ex.StackTrace.Substring(0, 1000);
                });
                embed.AddField(field =>
                {
                    field.Name = "Error Message";
                    field.Value = ex.Message;
                });
                embed.AddField(field =>
                {
                    field.Name = "Source Class";
                    field.Value = "AddAdminCommand";
                });
                await Context.Channel.SendMessageAsync("Es gab einen Datenbankfehler. Der Administrator kann nicht hinzugefügt werden. " +
                    "There was an error retrieving the email. An administrator can not be added.\n\r", embed: embed.Build());
            }
        }

        [Command("removeAdmin")]
        [Alias("latency")]
        [Summary("Removes an admin account from the system, who receives emails if an error was detected")]
        public async Task RemoveAdmin(string email = "")
        {
            try
            {
                Student? student = _studentRepository.FindWithEmail(email);
                if (student != null)
                {
                    _databaseErrorNotificationService.RemoveAdminMail(student.StudentEmail);
                    _studentRepository.SetStudentIsAdmin(student, false);

                    await ReplyAsync($"Der Student mit der Email {student.StudentEmail} wurde erfolglreich degradiert.\n\r" +
                        $"Student with email {student.StudentEmail} was succesfully removed as DiscordAdmin");
                    _logger.Info($"Student with email {student.StudentEmail} was succesfully removed as DiscordAdmin");
                }
                else
                {
                    await ReplyAsync($"Der Student mit der Email {email} konnte nicht als DiscordAdmin entfernt werden. Der Student existiert nicht.\n\r " +
                        $"Could not remove student with email {email} as DiscordAdmin. This student doesn't exists.");
                    _logger.Info($"Could not remove student with email {email} as DiscordAdmin");
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"There was an Error, due to a database exception. Stacktrace: {ex.Message}");
                var embed = new EmbedBuilder()
                {
                    Color = Color.Red
                };
                embed.WithTitle("*** Mail Service Error ***");
                embed.AddField(field =>
                {
                    field.Name = "Stacktrace";
                    field.Value = ex.StackTrace.Substring(0, 1000);
                });
                embed.AddField(field =>
                {
                    field.Name = "Error Message";
                    field.Value = ex.Message;
                });
                embed.AddField(field =>
                {
                    field.Name = "Source Class";
                    field.Value = "AddAdminCommand";
                });
                await Context.Channel.SendMessageAsync("Es gab einen Datenbankfehler. Der Administrator konnte nicht entfernt werden. " +
                    "There was an error retrieving the email. The administrator can not be removed.\n\r", embed: embed.Build());
            }
        }
    }
}
