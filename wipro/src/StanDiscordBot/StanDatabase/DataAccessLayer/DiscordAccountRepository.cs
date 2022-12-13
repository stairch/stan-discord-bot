using LinqToDB;
using StanDatabase.DTOs;
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
                return db.DiscordAccount
                    .LoadWith(da => da.Student)
                    .SingleOrDefault(da => da.AccountId == discriminaterValue && da.Username == username);
            }
        }

        public List<DiscordAccountsPerSemesterDTO> NumberOfDiscordAccountsPerSemester()
        {
            using (var db = new DbStan())
            {
                var query = from s in db.Student
                            join dc in db.DiscordAccount on s.StudentId equals dc.FkStudentId into joinGroup
                            from gr in joinGroup.DefaultIfEmpty()
                            group gr by s.Semester into g
                            select new DiscordAccountsPerSemesterDTO
                            {
                                Semester = g.Key,
                                AccountsCount = g.Count(stud => stud.FkStudentId != null)
                            };
                return query.ToList();
            }
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

        public bool IsStillStudying(DiscordAccount discordAccount)
        {
            using (var db = new DbStan())
            {
                return discordAccount.Student.StillStudying;
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

        public int Delete(int discordAccountId)
        {
            using (var db = new DbStan())
            {
                throw new NotImplementedException();
            }
        }
    }
}
