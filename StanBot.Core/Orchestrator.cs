namespace StanBot.Core
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class Orchestrator
    {
        private readonly BaseSocketClient discordClient;

        private readonly MailService mailService;

        private readonly VerificationCodeManager verificationCodeManager;

        public Orchestrator(BaseSocketClient discordClient, MailService mailService, VerificationCodeManager verificationCodeManager)
        {
            this.discordClient = discordClient;
            this.mailService = mailService;
            this.verificationCodeManager = verificationCodeManager;
        }

        public async Task LoginAsync(string token)
        {
            await this.discordClient.LoginAsync(TokenType.Bot, token);
            await this.discordClient.StartAsync();
        }

        public void RegisterNewUserListener()
        {
            this.discordClient.UserJoined += this.DiscordSocketClientOnUserJoined;
        }

        private async Task DiscordSocketClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            this.discordClient.MessageReceived += this.DiscordSocketClientOnMessageReceived;
            await socketGuildUser.SendMessageAsync(
                "Hello good friend. I am Stan and I would like to welcome you to the STAIR discord.\n\r"
                + "Since this discord server is only for students at the HSLU department of computer"
                + " science I need to verify that you are enrolled there. Please enter your stud.hslu.ch"
                + " mail adress and I will send you a mail with the verfication instructions.");
        }

        private async Task DiscordSocketClientOnMessageReceived(IMessage message)
        {
            Regex regex = new Regex("(\\w*.\\w*@stud.hslu.ch)");
            Match match = regex.Match(message.Content);
            if (match.Success)
            {
                int verificationCode = this.verificationCodeManager.CreateCodeForUser(message.Author.Id);

                string messageBody = $"Hello {message.Author.Username}\n\r"
                                     + $"Here is your verification code for the STAIR discord server: {verificationCode}\n\r"
                                     + $"If you have any questions about me, the discord server or about STAIR please don't "
                                     + $"hesitate to ask a STAIR member (marked green on discord).\n\r\n\r"
                                     + $"Kind regards\n"
                                     + $"Stan";

                this.mailService.SendMailTo(message.Content, "STAIR Discord Verification", messageBody);
            }
            else
            {
                await message.Channel.SendMessageAsync("Sorry but I couldn't find a mail adress in the correct format (stan.cactus@stud.hslu.ch). Please try again.");
            }
        }
    }
}