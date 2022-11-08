﻿using LinqToDB;
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
        // examples:
        // I.BA_MAT1.17
        // I.BA_NETW2_MM.21
        /// </summary>
        /// <param name="occasionNumber"></param>
        /// <returns></returns>
        public static string ExtractModuleShortname(string occasionNumber)
        {
            if (occasionNumber.Contains("ISA_") || occasionNumber.Contains("_ISA"))
            {
                string shortName = occasionNumber.Split('.')[1];
                string[] subName = shortName.Split('_');
                int indexOfIsaInName = Array.IndexOf(subName, "ISA");
                if (indexOfIsaInName == subName.Length - 1)
                {
                    shortName = subName.ElementAt(indexOfIsaInName - 1) + '_' + subName.ElementAt(indexOfIsaInName);
                }
                else
                {
                    shortName = subName.ElementAt(indexOfIsaInName) + '_' + subName.ElementAt(indexOfIsaInName + 1);
                }
                return shortName;
            }
            else
            {
                // TODO: maybe use full name as a help? Cut out string that differentiates in short name when the full name is the same
                string[] subName = occasionNumber.Split('.')[1].Split('_');
                // source: https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits-in-c-sharp
                if (subName.Length >= 3 && !subName[1].All(char.IsDigit) && !subName[2].All(char.IsDigit))
                {
                    return subName[1] + "_" + subName[2];
                }
                else if (!subName[1].All(char.IsDigit))
                {
                    return subName[1];
                }
                else // if (!subName[2].All(char.IsDigit))
                {
                    return subName[2];
                }
            }
        }
    }
}