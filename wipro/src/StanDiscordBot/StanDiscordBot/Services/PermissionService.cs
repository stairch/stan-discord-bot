using Discord;

namespace StanBot.Services
{
    public static class PermissionService
    {
        /// <summary>
        /// https://stackoverflow.com/questions/48832919/discord-net-bot-create-a-private-channel#:~:text=In%20order%20to%20make%20a,other%20roles%20in%20the%20server.
        /// </summary>
        /// <returns></returns>
        public static OverwritePermissions GetFullAdminPermissions()
        {
            return new OverwritePermissions(PermValue.Deny,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow, // This parameter is for the 'viewChannel' permission
                                            PermValue.Allow,
                                            PermValue.Deny,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Allow,
                                            PermValue.Deny);
        }
    }
}
