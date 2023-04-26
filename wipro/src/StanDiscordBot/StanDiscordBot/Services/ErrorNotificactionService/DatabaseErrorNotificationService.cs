using NLog;
using StanBot.Services.MailService;
using File = System.IO.File;

namespace StanBot.Services.ErrorNotificactionService
{
    public class DatabaseErrorNotificationService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string ADMIN_FILE = "C:\\Users\\localadmin\\stair-config\\stanBot\\discordAdmins.csv";
        private readonly IMailService _mailService;

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

            foreach (string email in ReadAdminMailsFromFile())
            {
                _mailService.SendMailToAsync(email, "Stan Discord Bot Database Error", message, 2);
            }
        }

        public bool AddAdminMail(string email)
        {
            if (!File.Exists(ADMIN_FILE))
            {
                string error = $"Error: File not found! Check your path. Path: {ADMIN_FILE}";
                _logger.Error(error);
                return false;
            }

            using (StreamWriter sw = File.AppendText(ADMIN_FILE))
            {
                sw.WriteLine(email);
            }
            return true;
        }

        public bool RemoveAdminMail(string email)
        {
            if (!File.Exists(ADMIN_FILE))
            {
                string error = $"Error: File not found! Check your path. Path: {ADMIN_FILE}";
                _logger.Error(error);
                return false;
            }

            IList<string> emailFromAdmins = new List<string>();
            using (StreamReader reader = new StreamReader(File.OpenRead(ADMIN_FILE)))
            {
                while (!reader.EndOfStream)
                {
                    string mail = GetCsvValuesOnNextLine(reader);
                    if (mail != email)
                    {
                        emailFromAdmins.Add(mail);
                        _logger.Debug(mail);
                    }
                }
            }
            
            File.WriteAllLines(ADMIN_FILE, emailFromAdmins);
            return true;
        }

        private IList<string> ReadAdminMailsFromFile()
        {
            if (!File.Exists(ADMIN_FILE))
            {
                string error = $"Error: File not found! Check your path. Path: {ADMIN_FILE}";
                _logger.Error(error);
                return new List<string>();
            }

            IList<string> emailFromAdmins = new List<string>();
            using (StreamReader reader = new StreamReader(File.OpenRead(ADMIN_FILE)))
            {
                while (!reader.EndOfStream)
                {
                    string mail = GetCsvValuesOnNextLine(reader);
                    emailFromAdmins.Add(mail);
                }
            }
            return emailFromAdmins;
        }
        
        private string GetCsvValuesOnNextLine(StreamReader reader)
        {
            string line = reader.ReadLine();
            line = line.Trim();
            return line;
        }
    }
}
