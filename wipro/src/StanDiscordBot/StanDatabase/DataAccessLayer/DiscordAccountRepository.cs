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

        public bool IsAdmin(string username)
        {
            using (var db = new DbStan())
            {
                return db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .Where(da => da.Username.Equals(username))
                    .Single()
                    .Student
                    .IsDiscordAdmin;
            }
        }

        public bool IsStudent(string username)
        {
            // TODO
            throw new NotImplementedException();
        }

        public bool IsExstudent(string username)
        {
            // TODO
            throw new NotImplementedException();
        }

        public House GetHouseFromStudent(string username)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
