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

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(Models.DiscordAccount.DiscordAccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkModuleId { get; set; }

        [Association(ThisKey = nameof(FkModuleId), OtherKey = nameof(Models.Module.ModuleId))]
        public Module Module { get; set; }

        public static DiscordAccountModule CreateNew(DiscordAccount discordAccount, Module module)
        {
            DiscordAccountModule accountModule = new DiscordAccountModule();
            accountModule.DiscordAccount = discordAccount;
            accountModule.FkDiscordAccountId = discordAccount.DiscordAccountId;
            accountModule.Module = module;
            accountModule.FkModuleId = module.ModuleId;
            accountModule.CreationDate = DateTime.Now;
            return accountModule;
        }
    }
}
