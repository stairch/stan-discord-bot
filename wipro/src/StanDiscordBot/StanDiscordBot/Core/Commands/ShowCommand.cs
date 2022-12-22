using Discord;
using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanBot.Services;
using StanBot.Services.ErrorNotificactionService;
using StanBot.Services.MailService;
using StanDatabase.DataAccessLayer;
using StanDatabase.Models;
using StanDatabase.Repositories;


namespace StanBot.Core.Commands
{
    public class ShowCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ModuleChannelService _moduleChannelService;
        private readonly DatabaseErrorNotificationService _databaseErrorNotificationService;

        private readonly IDiscordAccountModuleRepository _discordAccountModuleRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IStudentRepository _studentRepository;

        private readonly IMailService _mailService;

        public ShowCommand(
            IDiscordAccountModuleRepository discordAccountModuleRepository,
            IModuleRepository moduleRepository,
            IDiscordAccountRepository discordAccountRepository,
            IStudentRepository studentRepository,
            ModuleChannelService moduleChannelService,
            DatabaseErrorNotificationService databaseErrorNotificationService,
            IMailService mailService)
        {
            _discordAccountModuleRepository = discordAccountModuleRepository;
            _moduleRepository = moduleRepository;
            _discordAccountRepository = discordAccountRepository;
            _studentRepository = studentRepository;
            _moduleChannelService = moduleChannelService;
            _databaseErrorNotificationService = databaseErrorNotificationService;
            _mailService = mailService;
        }

        [Command("Show", false)]
        [RequireRole("student")]
        public async Task ShowModuleCommand([Remainder] string moduleName = "")
        {
            if (Context.Channel.Name.ToLower().Equals("registering"))
            {
                try
                {
                    if (_moduleRepository.DoesModuleExist(moduleName))
                    {
                        Module module =_moduleRepository.GetModuleByName(moduleName);
                        if (!_moduleChannelService.DoesModuleChannelExist(Context, module.ChannelName))
                        {
                            string logMessage = $"Creating text channel: {module.ChannelName}";
                            Console.WriteLine(logMessage);
                            _logger.Info(logMessage);
                            ICategoryChannel categoryChannel = Context.Guild.CategoryChannels
                                .SingleOrDefault(c => c.Name.Equals(module.DiscordCategory.DiscordCategoryName));
                            // create Discord category if it didn't exist
                            if (categoryChannel == null)
                            {
                                string categoryLogMessage = $"Creating category: {module.DiscordCategory.DiscordCategoryName}";
                                Console.WriteLine(categoryLogMessage);
                                _logger.Info(categoryLogMessage);
                                categoryChannel = await Context.Guild.CreateCategoryChannelAsync(module.DiscordCategory.DiscordCategoryName);
                            }
                            // create Discord channel if it didn't exist
                            // <https://stackoverflow.com/questions/67729674/how-to-find-a-channel-by-name-and-create-it-if-it-does-not-exist-in-discord-net>
                            var newChannel = await Context.Guild.CreateTextChannelAsync(module.ChannelName, tcp =>
                                {
                                    tcp.CategoryId = categoryChannel.Id;
                                }
                            );
                            // <https://stackoverflow.com/questions/67775430/discord-net-bot-how-to-create-a-channel-that-only-administrator-roles-have-acce>
                            await newChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(newChannel));
                        }

                        DiscordAccount discordAccount = _discordAccountRepository.GetAccount(Context.User.DiscriminatorValue, Context.User.Username);

                        if (discordAccount != null)
                        {
                            _studentRepository.AddModuleToUser(discordAccount, module);

                            _moduleChannelService.GiveUserAccessToModule(Context, Context.User, module);

                            IUserMessage reply = await ReplyAsync($"Success! You were added to the module channel: {moduleName}");
                            Thread.Sleep(5000);
                            Context.Message.DeleteAsync();
                            reply.DeleteAsync();
                        }
                        else
                        {
                            string errorMessage = $"User '{Context.User.Username}#{Context.User.Id}' tries to show module '{module.ChannelName}' but user wasn't found in the database!";
                            _logger.Error(errorMessage);
                            Console.WriteLine(errorMessage);
                            // TODO: inform admin per email.
                            await ReplyAsync("There was a server error. Please contact an admin!");
                        }
                    }
                    else
                    {
                        _logger.Warn($"Module not found in database while user tried showing it! Module name: {moduleName}");
                        await ReplyAsync("Module not found. Have you written it correctly? Please contact an admin if the error persists!");
                    }
                }
                catch (Exception ex)
                {
                    _databaseErrorNotificationService.SendDatabaseErrorToAdmins(ex, "ShowCommand");
                    _logger.Error($"There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {ex.Message}");
                    await Context.Channel.SendMessageAsync("Es gab einen Fehler bei der Abfragen des Modules. Ein Administrator wurde schon kontaktiert. " +
                        "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                        "There was an error retreiving the module list. An administrator has already been contacted. Please be patient and try again later.");
                }
            }
        }
    }
}
