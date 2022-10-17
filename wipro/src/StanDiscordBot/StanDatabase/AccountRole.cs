﻿using LinqToDB.Mapping;

namespace StanDatabase
{
    public class AccountRole
    {
        [Column, NotNull]
        public int FkDiscordAccountId { get; set; }

        [Association(ThisKey = nameof(FkDiscordAccountId), OtherKey = nameof(StanDatabase.DiscordAccount.AccountId))]
        public DiscordAccount DiscordAccount { get; set; }

        [Column, NotNull]
        public int FkDiscordRoleId { get; set; }

        [Association(ThisKey = nameof(FkDiscordRoleId), OtherKey = nameof(StanDatabase.DiscordRole.RoleId))]
        public DiscordRole DiscordRole { get; set; }
    }
}
