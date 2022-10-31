using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "DiscordAccountsDiscordRoles")]
    public class DiscordAccountDiscordRole
    {
        [Column, NotNull]
        public int FkDiscordAccountId { get; set; }

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(Models.DiscordAccount.AccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkDiscordRoleId { get; set; }

        [Association(ThisKey = nameof(FkDiscordRoleId), OtherKey = nameof(Models.DiscordRole.DiscordRoleId))]
        public DiscordRole DiscordRole { get; set; }
    }
}
