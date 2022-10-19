using LinqToDB.Mapping;

namespace StanDatabase
{
    [Table(Name = "DiscordAccountsModules")]
    public class DiscordAccountModule
    {
        [PrimaryKey, Identity]
        public int AccountModuleId { get; set; }

        [Column, NotNull]
        public DateTime CreationDate { get; set; }

        [Column, NotNull]
        public int FkDiscordAccountId { get; set; }

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(StanDatabase.DiscordAccount.AccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkModuleId { get; set; }

        [Association(ThisKey = nameof(FkModuleId), OtherKey = nameof(StanDatabase.Module.ModuleId))]
        public Module Module { get; set; }
    }
}
