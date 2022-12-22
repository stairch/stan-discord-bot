namespace StanBot.Core.Events.Messages
{
    public class VerificationCode
    {
        public ulong UserId { get; }

        public DateTime DateOfCreation { get; }

        public int Code { get; }

        public string Email { get; }

        public VerificationCode(ulong userId, int code, string email)
        {
            UserId = userId;
            DateOfCreation = DateTime.Now;
            Code = code;
            Email = email;
        }
    }
}
