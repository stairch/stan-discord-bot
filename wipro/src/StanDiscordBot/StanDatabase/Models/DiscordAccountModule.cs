using LinqToDB.Mapping;

namespace StanDatabase.Models
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

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(Models.DiscordAccount.AccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkModuleId { get; set; }

        [Association(ThisKey = nameof(FkModuleId), OtherKey = nameof(Models.Module.ModuleId))]
        public Module Module { get; set; }
    }
}
