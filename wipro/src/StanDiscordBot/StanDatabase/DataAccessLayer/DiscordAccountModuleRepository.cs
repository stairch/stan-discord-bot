using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordAccountModuleRepository : IDiscordAccountModuleRepository
    {
        private DiscordAccountRepository discordAccountRepository;

        private ModuleRepository moduleRepository;

        public DiscordAccountModuleRepository(
            DiscordAccountRepository discordAccountRepository,
            ModuleRepository moduleRepository)
        {
            this.discordAccountRepository = discordAccountRepository;
            this.moduleRepository = moduleRepository;
        }

        public void AddModuleToUser(string user, string moduleName)
        {
            using (var db = new DbStan())
            {
                // TODO /////////////////////////
                //DiscordAccount discordAccount = discordAccountRepository.
                //DiscordAccountModule discordAccountModule = new DiscordAccountModule(DateTime.Now, );
                // TODO: set module and account
                //discordAccountModule.AccountModuleId = db.Insert(discordAccountModule);
            }
        }
    }
}
