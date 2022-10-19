﻿using LinqToDB.Mapping;

namespace StanDatabase
{
    [Table(Name = "DiscordCategories")]
    public class DiscordCategory
    {
        [PrimaryKey, Identity]
        public int DiscordCategoryId { get; set; }

        [Column, NotNull]
        public string DiscordCategoryName { get; set; }
    }
}
