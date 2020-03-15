namespace StanBot.Core
{
    using System;

    public class VerificationCode
    {
        public VerificationCode(ulong userId, int code)
        {
            this.UserId = userId;
            this.DateOfCreation = DateTime.Now;
            this.Code = code;
        }

        public ulong UserId { get; }

        public DateTime DateOfCreation { get; }

        public int Code { get; }
    }
}