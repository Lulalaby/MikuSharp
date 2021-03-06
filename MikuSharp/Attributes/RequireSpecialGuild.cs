﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikuSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RequireSpecialGuild : CheckBaseAttribute
    {
        public ulong GuildId { get; }

        public RequireSpecialGuild(ulong guild)
        {
            this.GuildId = guild;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild.Id == GuildId)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }
    }
}
