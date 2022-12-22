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

        public bool IsCodeCorrectForUser(int verificationCode, ulong userId)
        {
            return this._openVerifications.Any(vc => vc.Code == verificationCode && vc.UserId == userId);
        }

        public string getEmaiForUser(int verificationCode, ulong userId)
        {
            VerificationCode? vc = _openVerifications.Find(vc => vc.UserId == userId && vc.Code == verificationCode);
            return vc != null ? vc.Email : "";
        }

        public void RemoveCodeForUser(ulong userId)
        {
            _openVerifications.RemoveAll(vc => vc.UserId == userId);
        }
    }
}
