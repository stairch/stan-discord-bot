using Discord;
using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanBot.Services;
using StanBot.Services.MailService;
using StanDatabase.DataAccessLayer;
using StanDatabase.Models;
using StanDatabase.Repositories;


namespace StanBot.Core.Commands
{
    public class ShowCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMailService _mailService = new MailService();

        [Command("Show", false)]
        [RequireRole("student")]
        public async Task ShowModuleCommand([Remainder] string moduleName = "")
        {
            if (Context.Channel.Name.ToLower().Equals("registering"))
            {
                IModuleRepository moduleRepository = new ModuleRepository(new DiscordCategoryRepository());
                IDiscordAccountRepository discordAccountRepository = new DiscordAccountRepository();
                if (moduleRepository.DoesModuleExist(moduleName))
                {
                    ModuleChannelService moduleChannelService = new ModuleChannelService();

                    Module module = moduleRepository.GetModuleByName(moduleName);
                    if (!moduleChannelService.DoesModuleChannelExist(Context, module.ChannelName))
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
                        //categoryChannel = Context.Guild.CategoryChannels
                        //    .SingleOrDefault(c => c.Name.Equals(module.DiscordCategory.DiscordCategoryName));
                        // create Discord channel if it didn't exist
                        // https://stackoverflow.com/questions/67729674/how-to-find-a-channel-by-name-and-create-it-if-it-does-not-exist-in-discord-net
                        var newChannel = await Context.Guild.CreateTextChannelAsync(module.ChannelName, tcp =>
                            {
                                tcp.CategoryId = categoryChannel.Id;
                                //tcp.PermissionOverwrites.Value = PermissionService.GetFullAdminPermissions();
                            }
                        );
                        // https://stackoverflow.com/questions/67775430/discord-net-bot-how-to-create-a-channel-that-only-administrator-roles-have-acce
                        //await newChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(0uL, ChannelPermissions.Text.RawValue));
                        await newChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(newChannel));
                    }

                    if (moduleChannelService.DoesModuleChannelExist(Context, moduleName))
                    {
                        DiscordAccount discordAccount = discordAccountRepository.GetDiscordAccountByName(Context.User.Username);

                        if (discordAccount != null)
                        {
                            IStudentRepository studentRepository = new StudentRepository();
                            studentRepository.AddModuleToUser(discordAccount, module);

                            moduleChannelService.GiveUserAccessToModule(Context, Context.User, module);

                            IUserMessage reply = await ReplyAsync($"Success! You were added to the module channel: {moduleName}");
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
                    else
                    {
                        string errorMessage = "Should have found module channel on Discord server but didn't!";
                        errorMessage += $" Expected module channel: {moduleName}";
                        _logger.Error(errorMessage);
                        Console.WriteLine(errorMessage);
                        // TODO: inform admin per email.
                        await ReplyAsync("There was a server error. Please contact an admin if the error persists!");
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
