using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordAccountRepository : IDiscordAccountRepository
    {
        public int Insert(DiscordAccount discordAccount)
        {
            using (var db = new DbStan())
            {
                return db.InsertWithInt32Identity(discordAccount);
            }
        }

        public int Update(DiscordAccount discordAccount)
        {
            throw new NotImplementedException();
        }

        public int Delete(int discordAccountId)
        {
            throw new NotImplementedException();
        }
    }
}
