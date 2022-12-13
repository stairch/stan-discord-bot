using LinqToDB;
using StanDatabase.DTOs;
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

        public int Insert(DiscordAccountModule discordAccountModule)
        {
            using (var db = new DbStan())
            {
                return db.InsertWithInt32Identity(discordAccountModule);
            }
        }
    }
}
