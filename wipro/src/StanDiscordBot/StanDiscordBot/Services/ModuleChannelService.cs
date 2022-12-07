using Discord;
using Discord.Commands;
using Discord.WebSocket;
using StanDatabase.Models;

namespace StanBot.Services
{
    public class ModuleChannelService
    {
        /// <summary>
        /// Adds user access to module channel on Discord server.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="module"></param>
        /// <returns>Returns true when it worked.</returns>
        public bool GiveUserAccessToModule(SocketCommandContext context, SocketUser user, Module module)
        {
            SocketGuildChannel socketGuildChannel = context.Guild.Channels
                .SingleOrDefault(c => c.Name.ToLower().Equals(
                    module.ChannelName.ToLower()
                )
            );
            if (socketGuildChannel != null)
            {
                socketGuildChannel.AddPermissionOverwriteAsync(user, OverwritePermissions.InheritAll);
                return true;
            }
            return false;
        }

        public void RemoveUserAccessToModule(string user, string moduleName)
        {
            // TODO

        }

        public bool DoesModuleChannelExist(string moduleName)
        {
            // TODO
            return false;
        }

        public void CreateModuleChannel(string moduleName)
        {
            // TODO

        }

        public void RemoveModuleChannel(string moduleName)
        {
            // TODO

        }
    }
}
