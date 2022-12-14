using StanBot.Services.MailService;

namespace StanBot.Services.ErrorNotificactionService
{
    public class DatabaseErrorNotificationService
    {
        private readonly IMailService _mailService;

        private IList<string> _emailFromAdmins = new List<string>();

        public DatabaseErrorNotificationService(IMailService mailService)
        {
            _mailService = mailService;
        }

        /// <summary>
        /// Sends an Email to all Discord Admins registered, with describtion of the database error.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="sourceClassName"></param>
        public void SendDatabaseErrorToAdmins(Exception exception, string sourceClassName)
        {
            string message = $"There was a database error.\n\rStacktrace:\n\r{exception.StackTrace}\n\rError Message: {exception.Message}\n\rSource Class: {sourceClassName}.";

            foreach (string email in _emailFromAdmins)
            {
                _mailService.SendMailToAsync(email, "Stan Discord Bot Database Error", message, 2);
            }
        }

        public void AddAdminMail(string email)
        {
            _emailFromAdmins.Add(email);
        }

        public void RemoveAdminMail(string email)
        {
            _emailFromAdmins.Remove(email);
        }
    }
}
