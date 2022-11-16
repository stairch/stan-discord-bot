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
        public int AccountId { get; set; }

        [Column]
        public int FkStudentId { get; set; }

        [Association(ThisKey = nameof(FkStudentId), OtherKey = nameof(Models.Student.StudentId))]
        public Student Student { get; set; }

        [Column]
        public DateTime VerifiedDate { get; set; }

        public DiscordAccount(string username, int accountId, int fkStudentId, DateTime verifiedDate)
        {
            Username = username;
            AccountId = accountId;
            FkStudentId = fkStudentId;
            VerifiedDate = verifiedDate;
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
