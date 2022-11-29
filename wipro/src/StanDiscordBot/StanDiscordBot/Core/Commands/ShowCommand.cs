using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanBot.Services;
using StanDatabase.DataAccessLayer;
using StanDatabase.Repositories;

namespace StanBot.Core.Commands
{
    public class ShowCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Command("Show", false)]
        [RequireRole("student")]
        public async Task ShowModuleCommand([Remainder] string moduleName = "")
        {
            await ReplyAsync($"moduleName: {moduleName}");
            IModuleRepository moduleRepository = new ModuleRepository(new DiscordCategoryRepository());
            if (moduleRepository.DoesModuleExist(moduleName))
            {
                await ReplyAsync($"moduleName: {Context.Message}");
                ModuleChannelService moduleChannelService = new ModuleChannelService();
                if (moduleChannelService.DoesModuleChannelExist(moduleName))
                {
                    // TODO: add link in DB
                    IStudentRepository studentRepository = new StudentRepository();
                    // TODO
                    //studentRepository.AddModuleToUser();
                    // TODO: give access on Discord
                    //moduleChannelService.GiveUserAccessToModule(Context.User, moduleName);
                    await ReplyAsync($"Success! You were added to the module channel: {moduleName}");
                    // TODO: remove message
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
    }
}
