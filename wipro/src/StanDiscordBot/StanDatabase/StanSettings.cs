namespace StanDatabase
{
    public static class StanSettings
    {
        // TODO: put these in a settings file and load them when needed

        // database configuration
        public static string DatabaseName = "StanDB";
        public static string ConnectionString = "Server=localhost;Port=3306;Database=standb;Uid=root;Pwd=12345678;charset=utf8;";

        // CSV configuration
        public static char Separator = ',';
        public static string EmailColumnNameInCsv = "E-Mail";
        public static string HouseColumnNameInCsv = "STAIR House";
        public static string SemesterColumnNameInCsv = "Semester";
        public static string ModuleShortnameInCsv = "Anlassnummer";
        public static string ModuleFullnameInCsv = "Anlassbezeichnung";
    }
}
