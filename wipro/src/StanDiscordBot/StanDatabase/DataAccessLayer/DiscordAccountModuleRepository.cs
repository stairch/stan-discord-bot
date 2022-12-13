using LinqToDB;
using StanDatabase.DTOs;
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

        public DiscordAccountModuleRepository()
        {
            this.discordAccountRepository = new DiscordAccountRepository();
            this.moduleRepository = new ModuleRepository(new DiscordCategoryRepository());
        }

        public List<MembersPerModuleDTO> NumberOfMembersPerModule(int limit = 10)
        {
            using (var db = new DbStan())
            {
                var query = from am in db.DiscordAccountModule
                            join m in db.Module on am.FkModuleId equals m.ModuleId
                            group m by m.ChannelName into g
                            orderby g.Count() descending
                            select new MembersPerModuleDTO
                            {
                                ModuleName = g.Key,
                                MemberCount = g.Count()
                            };
                return query.Take(limit).ToList();
            }
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
