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

        [Column, NotNull]
        public string ActivationCode { get; set; }

        [Column]
        public DateTime VerifiedDate { get; set; }

        [Column]
        public DateTime RegisterDate { get; set; }
    }
}
