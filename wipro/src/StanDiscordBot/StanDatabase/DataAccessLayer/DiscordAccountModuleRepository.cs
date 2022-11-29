using StanDatabase.DTOs;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordAccountModuleRepository : IDiscordAccountModuleRepository
    {
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
    }
}
