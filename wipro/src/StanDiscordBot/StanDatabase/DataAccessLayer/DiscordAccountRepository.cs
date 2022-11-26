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

        public bool DoesDiscordAccountExist(int discriminatorValue, string username)
        {
            using(var db = new DbStan())
            {
                return db.DiscordAccount.Any(da => da.AccountId == discriminatorValue && da.Username == username);
            }
        }

        public DiscordAccount? GetAccount(int discriminaterValue, string username)
        {
            using(var db = new DbStan())
            {
                return db.DiscordAccount.SingleOrDefault(da => da.AccountId == discriminaterValue && da.Username == username);
            }
        }
    }
}
