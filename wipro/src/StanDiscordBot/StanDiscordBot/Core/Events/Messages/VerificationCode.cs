using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanBot.Core.Events.Messages
{
    public class VerificationCode
    {
        public ulong UserId { get; }

        public DateTime DateOfCreation { get; }

        public int Code { get; }

        public VerificationCode(ulong userId, int code)
        {
            this.UserId = userId;
            this.DateOfCreation = DateTime.Now;
            this.Code = code;
        }
    }
}
