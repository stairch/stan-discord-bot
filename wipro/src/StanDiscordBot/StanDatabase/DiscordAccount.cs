using LinqToDB.Mapping;

namespace StanDatabase
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
        public DateTime VerifiedDate { get; set; }

        public DiscordAccount(int discordAccountId, string username, int accountId, DateTime verifiedDate)
        {
            DiscordAccountId = discordAccountId;
            Username = username;
            AccountId = accountId;
            VerifiedDate = verifiedDate;
        }

        public DiscordAccount(string username, int accountId, string activationCode)
        {
            Username = username;
            AccountId = accountId;
        }
    }
}
