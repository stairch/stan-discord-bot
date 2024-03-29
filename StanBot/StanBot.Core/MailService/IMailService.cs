﻿using System.Threading.Tasks;

namespace StanBot.Core.MailService
{
    public interface IMailService
    {
        Task SendMailToAsync(string mailAdress, string subject, string messageBody);

        Task Initialize(string fromMailAddress, string fromName, string appId, string[] scopes);
    }
}