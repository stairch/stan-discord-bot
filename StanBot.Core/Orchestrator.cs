using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace StanBot.Core
{
    public class Orchestrator
    {
        private readonly Communicator communicator;
        private DiscordClient discordClient;

        public Orchestrator(Communicator communicator, DiscordClient discordClient)
        {
            this.communicator = communicator;
            this.discordClient = discordClient;
        }

        public async Task InitializeDiscordAsync(string discordApplicationToken, string guildName, string announcementRoleName, string studentRoleName)
        {
            this.discordClient.Initialize(guildName, announcementRoleName, studentRoleName);
            await this.discordClient.LoginAsync(discordApplicationToken);
            this.discordClient.RegisterUserJoinedListener(this.DiscordSocketClientOnUserJoined);
            this.discordClient.RegisterMessageReceivedListener(this.communicator.DiscordClientOnMessageReceived);
        }

        private async Task DiscordSocketClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            await socketGuildUser.SendMessageAsync(
                "Hello good friend. \n\r"
                + "Ich bin Stan und heisse dich herzlich Willkommen auf dem STAIR Discord Server.\n"
                + "Da der Discord Server nur für Studenten der HSLU Informatik ist, muss ich verifizieren, "
                + "dass du dort immatrikuliert bist. Bitte sende mir dafür deine stud.hslu.ch Mail Adresse zu. "
                + "Danach werde ich dir ein Mail mit einem Code und weiteren Instruktionen schicken. "
                + "Mit dem weiteren Verweilen auf dem Server akzeptierst du die folgenden Regeln.\n\r"
                + "**Regeln** - ja, leider\n\r"
                + "\t1) Anstandsregeln gelten auch online, und mit dem Beitreten auf dem Server bestätigt ihr, dass ihr sie kennt und anwendet\n"
                + "\t2) Gesunder Menschenverstand wird vorausgesetzt\n\r"
                + "\tWir gehen davon aus, dass dies ausreichend ist.\n"
                + "\tSollten die Regeln angepasst werden müssen, wird darüber im Channel #stair-announcements informiert - mit dem Verbleib auf dem Server gelten diese als akzeptiert.\n\r\n\r"
                + "I am Stan and I would like to welcome you to the STAIR discord.\n"
                + "Since this discord server is only for students at the HSLU department of computer"
                + " science I need to verify that you are enrolled there. Please enter your stud.hslu.ch"
                + " mail adress and I will send you a mail with the verification instructions. Furthermore,"
                + " you'll accept the following rules by staying on the server and verifying your identity.\n\r"
                + "**Rules** - shame, we know\n\r"
                + "\t1) Decency rules apply online as well. By entering this server, you confirm that you not only know but also comply with these rules\n"
                + "\t2) Common sense is assumed\n\r"
                + "\tWe assume that the rules above suffice.\n"
                + "\tShould it be necessary to adapt the rules, you accept these by remaining on the server. Such a change will be published in #stair-announcements.");
        }
    }
}