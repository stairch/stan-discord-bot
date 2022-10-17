﻿using LinqToDB.Mapping;

namespace StanDatabase
{
    [Table(Name = "DiscordRoles")]
    public class DiscordRole
    {
        [PrimaryKey, Identity]
        public int RoleId { get; set; }

        [Column, NotNull]
        public string RoleName { get; set; }
    }
}
