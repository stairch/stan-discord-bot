using LinqToDB;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordAccountDiscordRoleRepository : IDiscordAccountDiscordRoleRepository
    {
        public List<string> getRolesForAccount(int discordAccountId)
        {
            using (var db = new DbStan())
            {
                var query = from ar in db.AccountRole
                            where ar.FkDiscordAccountId == discordAccountId
                            select ar.DiscordRole.RoleName;

                return query.ToList();
            }
        }
    }
}
