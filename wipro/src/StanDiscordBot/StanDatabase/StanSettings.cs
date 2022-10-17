using LinqToDB.Mapping;

namespace StanDatabase
{
    public static class StanSettings
    {
        // TODO: put these in a settings file and load them when needed

        // database configuration
        public static string DatabaseName = "StanDB";

        // CSV configuration
        public static char Separator = ',';
        public static string emailColumnNameInCsv = "E-Mail";
        public static string houseColumnNameInCsv = "STAIR House";
        public static string semesterColumnNameInCsv = "Semester";
    }
}
