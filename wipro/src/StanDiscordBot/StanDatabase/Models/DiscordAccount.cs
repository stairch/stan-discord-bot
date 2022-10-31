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

        [Column, NotNull]
        public string ActivationCode { get; set; }

        [Column]
        public DateTime VerifiedDate { get; set; }

        [Column]
        public DateTime RegisterDate { get; set; }

        public DiscordAccount(int discordAccountId, string username, int accountId, string activationCode, DateTime verifiedDate, DateTime registerDate)
        {
            DiscordAccountId = discordAccountId;
            Username = username;
            AccountId = accountId;
            ActivationCode = activationCode;
            VerifiedDate = verifiedDate;
            RegisterDate = registerDate;
        }

        public DiscordAccount(string username, int accountId, string activationCode)
        {
            Username = username;
            AccountId = accountId;
            ActivationCode = activationCode;
        }
    }
}
