namespace StanDatabase
{
    public class Module
    {
        public int ModuleId { get; set; }

        /// <summary>
        /// Is the same as short module name
        /// </summary>
        public string ChannelName { get; set; }

        public string FullModuleName { get; set; }

        // TODO: add FkCategoryId
    }
}
