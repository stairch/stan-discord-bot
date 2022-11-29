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
                return db.DiscordAccount.SingleOrDefault(da => da.AccountId == discriminaterValue && da.Username == username);
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
    }
}
