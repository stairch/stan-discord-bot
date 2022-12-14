using Discord;
using Discord.Commands;
using Discord.Interactions;
using NLog;
using StanBot.Services;
using StanBot.Services.MailService;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanBot.Core.Commands
{
    public class HideCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ModuleChannelService _moduleChannelService;

        private readonly IDiscordAccountModuleRepository _discordAccountModuleRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IStudentRepository _studentRepository;

        private readonly IMailService _mailService;

        public HideCommand(
            IDiscordAccountModuleRepository discordAccountModuleRepository,
            IModuleRepository moduleRepository,
            IDiscordAccountRepository discordAccountRepository,
            IStudentRepository studentRepository,
            ModuleChannelService moduleChannelService,
            IMailService mailService)
        {
            _discordAccountModuleRepository = discordAccountModuleRepository;
            _moduleRepository = moduleRepository;
            _discordAccountRepository = discordAccountRepository;
            _studentRepository = studentRepository;
            _moduleChannelService = moduleChannelService;
            _mailService = mailService;
        }

        [Command("Hide", false)]
        [RequireRole("student")]
        public async Task HideModuleCommand([Remainder] string moduleName = "")
        {
            if (Context.Channel.Name.ToLower().Equals("registering"))
            {
                if (_moduleRepository.DoesModuleExist(moduleName))
                {
                    Module module = _moduleRepository.GetModuleByName(moduleName);
                    if (_moduleChannelService.DoesModuleChannelExist(Context, moduleName))
                    {
                        DiscordAccount discordAccount = _discordAccountRepository.GetAccount((int)Context.User.Id, Context.User.Username);

                        if (discordAccount != null)
                        {
                            _studentRepository.RemoveUserFromModule(discordAccount, module);

                            _moduleChannelService.RemoveUserAccessToModule(Context, Context.User, module);

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
