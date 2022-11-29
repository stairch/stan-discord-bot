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
            // TODO
            throw new NotImplementedException();
        }

        public int Delete(int discordAccountId)
        {
            // TODO
            throw new NotImplementedException();
        }

        public bool IsAdmin(string username)
        {
            using (var db = new DbStan())
            {
                return db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .Single(da => da.Username.Equals(username))
                    .Student
                    .IsDiscordAdmin;
            }
        }

        public bool IsStillStudying(string username)
        {
            using (var db = new DbStan())
            {
                DiscordAccount? discordAccount = db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .SingleOrDefault(da => da.Username.Equals(username));
                if (discordAccount != null)
                {
                    return discordAccount
                        .Student
                        .StillStudying;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsExstudent(string username)
        {
            using (var db = new DbStan())
            {
                DiscordAccount? discordAccount = db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .SingleOrDefault(da => da.Username.Equals(username));
                if (discordAccount != null)
                {
                    return !discordAccount
                        .Student
                        .StillStudying;
                }
                else
                {
                    return false;
                }
            }
        }

        public House GetHouseFromStudent(string username)
        {
            using (var db = new DbStan())
            {
                DiscordAccount? discordAccount = db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .LoadWith(da => da.Student.House)
                    .SingleOrDefault(da => da.Username.Equals(username));
                if (discordAccount != null)
                {
                    return discordAccount.Student.House;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
