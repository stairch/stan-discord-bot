namespace StanBot.Core.Events.Messages
{
    public class VerificationCodeManager
    {
        private readonly List<VerificationCode> _openVerifications = new List<VerificationCode>();

        public int CreateCodeForUser(ulong userId, string email)
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000);
            VerificationCode verificationCode = new VerificationCode(userId, code, email);
            _openVerifications.Add(verificationCode);
            return code;
        }
    }
}
