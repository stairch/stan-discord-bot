using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanBot.Services;
using StanBot.Services.ErrorNotificactionService;
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

        public ShowCommand(
            IDiscordAccountModuleRepository discordAccountModuleRepository,
            IModuleRepository moduleRepository,
            IDiscordAccountRepository discordAccountRepository,
            ModuleChannelService moduleChannelService,
            DatabaseErrorNotificationService databaseErrorNotificationService) 
        { 
            _discordAccountModuleRepository = discordAccountModuleRepository;
            _moduleRepository = moduleRepository;
            _discordAccountRepository = discordAccountRepository;
            _moduleChannelService = moduleChannelService;
            _databaseErrorNotificationService = databaseErrorNotificationService;
        }

        [Command("Show", false)]
        [RequireRole("student")]
        public async Task ShowModuleCommand([Remainder] string moduleName = "")
        {
            try
            {
                await ReplyAsync($"moduleName: {moduleName}");
                if (_moduleRepository.DoesModuleExist(moduleName))
                {
                    await ReplyAsync($"moduleName: {Context.Message}");
                    if (_moduleChannelService.DoesModuleChannelExist(moduleName))
                    {
                        DiscordAccount? discordAccount = _discordAccountRepository.GetAccount(Context.User.DiscriminatorValue, Context.User.Username);
                        Module? module = _moduleRepository.GetModuleByName(moduleName);

                        if (discordAccount != null && module != null)
                        {
                            _discordAccountModuleRepository.Insert(DiscordAccountModule.CreateNew(discordAccount, module));
                            _moduleChannelService.GiveUserAccessToModule(Context, Context.User, module);
                            await ReplyAsync($"Success! You were added to the module channel: {moduleName}");
                            Thread.Sleep(5000);
                        }

                        // TODO: delete message
                    }
                    else
                    {
                        string errorMessage = "Should have found module channel on Discord server but didn't!";
                        errorMessage += $" Expected module channel: {moduleName}";
                        _logger.Error(errorMessage);
                        Console.WriteLine(errorMessage);
                        // TODO: inform admin per email.
                        await ReplyAsync("Module not found. Have you written it correctly? Please contact an admin if the error persists!");
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
