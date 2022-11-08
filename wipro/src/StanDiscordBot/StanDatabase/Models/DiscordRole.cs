﻿using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "DiscordRoles")]
    public class DiscordRole
    {
        [PrimaryKey, Identity]
        public int DiscordRoleId { get; set; }

        [Column, NotNull]
        public string RoleName { get; set; }
    }
}