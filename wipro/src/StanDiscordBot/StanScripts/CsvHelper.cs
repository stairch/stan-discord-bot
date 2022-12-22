using StanDatabase;

namespace StanScripts
{
    internal static class CsvHelper
    {
        public static string[] GetCsvValuesOnNextLine(StreamReader reader)
        {
            string line = reader.ReadLine();
            line = line.Trim();
            string[] values = line.Split(StanDatabaseConfigLoader.Get().Separator);
            return values;
        }
    }
}
