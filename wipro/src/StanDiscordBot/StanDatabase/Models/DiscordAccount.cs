using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "DiscordAccounts")]
    public class DiscordAccount
    {
        [PrimaryKey, Identity]
        public int DiscordAccountId { get; set; }

        [Column, NotNull]
        public string Username { get; set; }

        /// <summary>
        /// Describes the Id given by discord that is seen next to the name
        /// </summary>
        [Column, NotNull]
        public long AccountId { get; set; }

        [Column]
        public int FkStudentId { get; set; }

        [Association(ThisKey = nameof(FkStudentId), OtherKey = nameof(Models.Student.StudentId))]
        public Student Student { get; set; }

        [Column]
        public DateTime VerifiedDate { get; set; }

        public static DiscordAccount CreateNew(string username, long accountId, Student student)
        {
            DiscordAccount account = new DiscordAccount();
            account.Username = username;
            account.AccountId = accountId;
            account.Student = student;
            account.FkStudentId = student.StudentId;
            account.VerifiedDate = DateTime.Now;
            return account;
        }

        public override string ToString()
        {
            return $"{base.ToString()}[" +
                $"{nameof(DiscordAccountId)}: {DiscordAccountId}, " +
                $"{nameof(Username)}: {Username}, " +
                $"{nameof(AccountId)}: {AccountId}, " +
                $"{nameof(FkStudentId)}: {FkStudentId}, " +
                $"{nameof(VerifiedDate)}: {VerifiedDate}" +
                $"]";
        }
    }
}
