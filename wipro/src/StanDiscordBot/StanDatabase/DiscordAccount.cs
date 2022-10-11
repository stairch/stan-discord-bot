namespace StanDatabase
{
    public class DiscordAccount
    {
        public string Username { get; set; }

        public int AccountId { get; set; }

        public string ActivationCode { get; set; }

        public DateTime VerifiedDate { get; set; }

        public DateTime RegisterDate { get; set; }
    }
}
