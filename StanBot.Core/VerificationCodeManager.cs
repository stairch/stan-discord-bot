namespace StanBot.Core
{
    using System;
    using System.Collections.Generic;

    public class VerificationCodeManager
    {
        private readonly List<VerificationCode> verificationCodes = new List<VerificationCode>();

        public int CreateCodeForUser(ulong userId)
        {
            Random generator = new Random();
            int code = generator.Next(100000, 1000000);
            VerificationCode verificationCode = new VerificationCode(userId, code);
            this.verificationCodes.Add(verificationCode);
            return code;
        }
    }
}