using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "DiscordAccountsDiscordRoles")]
    public class DiscordAccountDiscordRole
    {
        [PrimaryKey, Identity]
        public int DiscordAccountDiscordRoleId { get; set; }

        [Column, NotNull]
        public int FkDiscordAccountId { get; set; }

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(Models.DiscordAccount.DiscordAccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkDiscordRoleId { get; set; }

        [Association(ThisKey = nameof(FkDiscordRoleId), OtherKey = nameof(Models.DiscordRole.DiscordRoleId))]
        public DiscordRole DiscordRole { get; set; }

        public static DiscordAccountDiscordRole CreateNew(DiscordAccount account, DiscordRole role)
        {
            DiscordAccountDiscordRole accountRole = new DiscordAccountDiscordRole();
            accountRole.DiscordAccount = account;
            accountRole.FkDiscordAccountId = account.DiscordAccountId;
            accountRole.DiscordRole = role;
            accountRole.FkDiscordRoleId= role.DiscordRoleId;
            return accountRole;
        }
    }
}
