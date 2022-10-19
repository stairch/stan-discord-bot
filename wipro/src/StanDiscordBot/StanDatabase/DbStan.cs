using LinqToDB;

namespace StanDatabase
{
    public class DbStan : LinqToDB.Data.DataConnection
    {
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
            : base(StanSettings.DatabaseName)
        {
        }

        public static void CreateDatabase()
        {
            //LinqToDB.DataProvider.MySql.MySqlTools.CreateDatabase();
        }
    }
}
