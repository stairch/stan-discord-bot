using LinqToDB;
using StanDatabase.Models;

namespace StanDatabase
{
    public class DbStan : LinqToDB.Data.DataConnection
    {
        // TODO: load this from settings file
        private const string connectionString = "Server=localhost;Port=3306;Database=standb;Uid=root;Pwd=12345678;charset=utf8;";

        public ITable<DiscordAccountModule> AccountModule => this.GetTable<DiscordAccountModule>();

        public ITable<DiscordAccountDiscordRole> AccountRole => this.GetTable<DiscordAccountDiscordRole>();

        public ITable<DiscordAccount> DiscordAccount => this.GetTable<DiscordAccount>();

        public ITable<DiscordCategory> DiscordCategory => this.GetTable<DiscordCategory>();

        public ITable<DiscordRole> DiscordRole => this.GetTable<DiscordRole>();

        public ITable<House> House => this.GetTable<House>();

        public ITable<Module> Module => this.GetTable<Module>();

        public ITable<Student> Student => this.GetTable<Student>();

        // TODO: check for singleton
        public DbStan()
            // possible providers list: https://github.com/linq2db/linq2db/blob/master/Source/LinqToDB/ProviderName.cs
            : base("MySql", connectionString)
        {
        }
    }
}
