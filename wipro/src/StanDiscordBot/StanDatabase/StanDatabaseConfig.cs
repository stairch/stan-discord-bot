namespace StanDatabase
{
    public class StanDatabaseConfig
    {
        public string DatabaseName { get; set; }

        public string ConnectionString { get; set; }

        public char Separator { get; set; }

        public string EmailColumnNameInCsv { get; set; }

        public string HouseColumnNameInCsv { get; set; }

        public string SemesterColumnNameInCsv { get; set; }

        public string ModuleShortnameInCsv { get; set; }

        public string ModuleFullnameInCsv { get; set; }
    }
}