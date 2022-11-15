using LinqToDB;
using System.Linq;

namespace StanDatabase.Util
{
    public static class ModuleUtil
    {
        public static bool IsModuleOccasionNumberValid(string moduleName)
        {
            return moduleName.StartsWith("I.") &&
                moduleName.Contains('_');
        }

        /// <summary>
        /// occasion number = Anlassnummer
        /// examples:
        /// I.BA_MAT1.17
        /// I.BA_NETW2_MM.21
        /// </summary>
        /// <param name="occasionNumber"></param>
        /// <returns></returns>
        public static string ExtractModuleShortname(string occasionNumber)
        {
            List<string> subName = occasionNumber.Split('.')[1].Split('_').ToList();
            // Can't remove them always since then the index of the element would change
            if (subName[1] == "EVA" || subName[1] == "ISA")
            {
                subName.RemoveAt(1);
            }

            if (subName.Count == 1)
            {
                return subName[0];
            }
            // source: https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits-in-c-sharp
            else if (!subName[1].All(char.IsDigit))
            {
                return subName[1];
            }
            else
            {
                return subName[2];
            }
        }
    }
}
