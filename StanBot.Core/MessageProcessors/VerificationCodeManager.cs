using System;
using System.Collections.Generic;
using System.Linq;

namespace StanBot.Core.MessageProcessors
{
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

        public bool IsCodeCorrectForUser(int verificationCode, ulong userId)
        {
            return this.verificationCodes.Any(vc => vc.Code == verificationCode && vc.UserId == userId);
        }
    }
}