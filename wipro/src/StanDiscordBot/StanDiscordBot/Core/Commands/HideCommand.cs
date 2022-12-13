using Discord;
using Discord.API;
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
    public class HideCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Command("Hide", false)]
        [RequireRole("student")]
        public async Task HideModuleCommand([Remainder] string moduleName = "")
        {
            if (Context.Channel.Name.ToLower().Equals("registering"))
            {
                IModuleRepository moduleRepository = new ModuleRepository(new DiscordCategoryRepository());
                IDiscordAccountRepository discordAccountRepository = new DiscordAccountRepository();
                if (moduleRepository.DoesModuleExist(moduleName))
                {
                    ModuleChannelService moduleChannelService = new ModuleChannelService();

                    Module module = moduleRepository.GetModuleByName(moduleName);
                    if (moduleChannelService.DoesModuleChannelExist(Context, moduleName))
                    {
                        DiscordAccount discordAccount = discordAccountRepository.GetAccount((int)Context.User.Id, Context.User.Username);

                        if (discordAccount != null)
                        {
                            IStudentRepository studentRepository = new StudentRepository();
                            studentRepository.RemoveUserFromModule(discordAccount, module);

                            moduleChannelService.RemoveUserAccessToModule(Context, Context.User, module);

                            IUserMessage reply = await ReplyAsync($"Success! You were removed from the module channel: {moduleName}");
                            Thread.Sleep(5000);
                            Context.Message.DeleteAsync();
                            reply.DeleteAsync();
                        }
                        else
                        {
                            string errorMessage = $"User '{Context.User.Username}' tries to show module '{module.ChannelName}' but wasn't found in the database!";
                            _logger.Error(errorMessage);
                            Console.WriteLine(errorMessage);
                            // TODO: inform admin per email.
                            await ReplyAsync("There was a server error. Please contact an admin!");
                        }
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
}
