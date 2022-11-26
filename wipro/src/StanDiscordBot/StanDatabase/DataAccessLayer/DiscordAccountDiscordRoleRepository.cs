using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordAccountDiscordRoleRepository : IDiscordAccountDiscordRoleRepository
    {
        public int Insert(DiscordAccountDiscordRole discordAccountDiscordRole)
        {
            using(var db = new DbStan())
            {
                return db.InsertWithInt32Identity(discordAccountDiscordRole);
            }
        }

        public List<string> getRolesForAccount(int discordAccountId)
        {
            using (var db = new DbStan())
            {
                var query = from ar in db.DiscordAccountDiscordRole
                            where ar.FkDiscordAccountId == discordAccountId
                            select ar.DiscordRole.DiscordRoleName;

                return query.ToList();
            }
        }
    }
}
