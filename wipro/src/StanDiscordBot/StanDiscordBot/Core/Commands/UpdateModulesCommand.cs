using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanDatabase.Repositories;

namespace StanBot.Core.Commands
{
    public class UpdateModulesCommand : ModuleBase<SocketCommandContext>
    {

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IDiscordCategoryRepository _discordCategoryRepository;

        private IModuleRepository _moduleRepository;

        public UpdateModulesCommand(IDiscordCategoryRepository discordCategoryRepository, IModuleRepository moduleRepository)
        {
            _discordCategoryRepository = discordCategoryRepository;
            _moduleRepository = moduleRepository;
        }

        [Command("updateModules", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Updates the modules channels. Removes the old ones and creates the new ones when necessary.")]
        public async Task UpdateModules([Remainder] string command = "")
        {
            if (Context.Channel.Name.Equals("bot-commands"))
            {
                _logger.Info($"User '{Context.User.Username}' started updating module channels.");
                await ReplyAsync("Start updating module channels");
                int numberOfUpdatedModuleChannels = 0;
                try
                {
                    foreach (var category in Context.Guild.CategoryChannels.Where(c => _discordCategoryRepository.DoesCategoryExist(c.Name)))
                    {
                        foreach (var channel in category.Channels.Where(c => !_moduleRepository.DoesModuleExist(c.Name)))
                        {
                            channel.DeleteAsync();
                            numberOfUpdatedModuleChannels++;
                        }
                    }
                    string logMessage = $"Finished updating module channels. Number of channels deleted: {numberOfUpdatedModuleChannels}";
                    await ReplyAsync(logMessage);
                    _logger.Info(logMessage);
                }
                catch (Exception ex)
                {
                    string logMessage = $"Error occured while updating module channels. {ex.Message}";
                    await ReplyAsync(logMessage);
                    _logger.Error(logMessage);
                }
            }
            else
            {
                _logger.Warn($"User '{Context.User.Username}' tried to update modules in wrong channel '{Context.Channel}'");
            }
        }
    }
}
