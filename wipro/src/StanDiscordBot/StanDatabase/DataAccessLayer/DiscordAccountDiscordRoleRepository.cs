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

        public void ChangeStillStudyingRole(int discordAccountId, int oldRoleId, DiscordRole newRole)
        {
            using (var db = new DbStan())
            {
                db.DiscordAccountDiscordRole
                    .Where(ar => ar.FkDiscordAccountId == discordAccountId && ar.FkDiscordRoleId == oldRoleId)
                    .Set(ar => ar.DiscordRole, newRole)
                    .Set(ar => ar.FkDiscordRoleId, newRole.DiscordRoleId)
                    .Update();
            }
        }

        public List<DiscordRole> GetRolesForAccount(int discordAccountId)
        {
            using (var db = new DbStan())
            {
                var query = from ar in db.DiscordAccountDiscordRole
                            where ar.FkDiscordAccountId == discordAccountId
                            select ar.DiscordRole;

                return query.ToList();
            }
        }

        public void Delete(int discordAccountId, int discordRoleId)
        { 
            using (var db = new DbStan())
            {
                db.DiscordAccountDiscordRole
                    .Where(dr => dr.FkDiscordAccountId == discordAccountId && dr.FkDiscordRoleId == discordRoleId)
                    .Delete();
            }
        }

        public DiscordRole? GetHouseRoleForAccount(int discordAccountId)
        {
            using (var db = new DbStan())
            {
                DiscordAccountDiscordRole? accountRole = db.DiscordAccountDiscordRole
                    .LoadWith(dr => dr.DiscordRole)
                    .Where(dr => dr.FkDiscordAccountId == discordAccountId)
                    .SingleOrDefault(dr => dr.DiscordRole.DiscordRoleName.StartsWith("House"));

                if(accountRole != null)
                {
                    return accountRole.DiscordRole;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
