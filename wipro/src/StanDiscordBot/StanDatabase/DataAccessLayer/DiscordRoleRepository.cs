using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordRoleRepository : IDiscordRoleRepository
    {
        public DiscordRole GetRoleByName(string roleName)
        {
            using(var db = new DbStan())
            {
                return db.DiscordRole.Single(r => r.DiscordRoleName == roleName);
            }
        }
    }
}
