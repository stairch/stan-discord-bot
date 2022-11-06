namespace StanDatabase.Util
{
    public static class StudentUtil
    {
        public static bool IsStudentEmailFormatValid(string email)
        {
            string hsluStudentEmailUrl = "stud.hslu.ch";
            string hsluEmailUrl = "hslu.ch";
            return (email.EndsWith($"@{hsluStudentEmailUrl}") || email.EndsWith($"@{hsluEmailUrl}")) &&
                // not checking for name format because it can be different when two have the same name
                email.Count(c => c == '@') == 1;
        }
    }
}
