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

        [Column, NotNull]
        public int ActivationCode { get; set; }

        [Column]
        public DateTime VerifiedDate { get; set; }

        [Column]
        public DateTime RegisterDate { get; set; }

        public DiscordAccount(string username, int accountId, int activationCode, int fkStudentId)
        {
            Username = username;
            AccountId = accountId;
            ActivationCode = activationCode;
            RegisterDate = DateTime.Now;
            FkStudentId = fkStudentId;
        }

        public override string ToString()
        {
            return $"{base.ToString()}[" +
                $"{nameof(DiscordAccountId)}: {DiscordAccountId}, " +
                $"{nameof(Username)}: {Username}, " +
                $"{nameof(AccountId)}: {AccountId}, " +
                $"{nameof(FkStudentId)}: {FkStudentId}, " +
                $"{nameof(ActivationCode)}: {ActivationCode}" +
                $"{nameof(VerifiedDate)}: {VerifiedDate}" +
                $"{nameof(RegisterDate)}: {RegisterDate}" +
                $"]";
        }
    }
}
