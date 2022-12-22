namespace StanDatabase.DTOs
{
    public class DiscordAccountsPerSemesterDTO
    {
        private int _semester;
        private int _accountsCount;

        public int Semester {
            get { return _semester; }
            set { _semester = value; }
        }

        public int AccountsCount {
            get { return _accountsCount; }
            set { _accountsCount = value; }
        }
    }
}
