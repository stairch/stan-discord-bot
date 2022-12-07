using Discord.Commands;
using Discord.Interactions;

namespace StanBot.Core.Commands
{
    public class UpdateModulesCommand : ModuleBase<SocketCommandContext>
    {

        [Command("updateModules", true)]
        // TODO: go to DB to check permissions
        // This seems to not be possible. Maybe this shouldn't be about the db but just about the roles?
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Updates the modules channels. Removes the old ones and creates the new ones when necessary.")]
        public async Task UpdateModules([Remainder] string command = "")
        {
            await ReplyAsync("Received message: " + command);
            await ReplyAsync("test");
        }
    }
}
