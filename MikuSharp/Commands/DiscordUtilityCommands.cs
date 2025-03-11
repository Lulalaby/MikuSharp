using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("discord", "Discord Utilities")]
internal class DiscordUtilityCommands : ApplicationCommandsModule
{
	[SlashCommand("avatar", "Get the avatar of someone or yourself")]
	public static async Task GetAvatarAsync(InteractionContext ctx, [Option("user", "User to get the avatar from")] DiscordUser? user = null)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(user is not null
			? user.AvatarUrl
			: ctx.User.AvatarUrl).Build()));

	[SlashCommand("guild_avatar", "Get the guild avatar of someone or yourself")]
	public static async Task GetGuildAvatarAsync(InteractionContext ctx, [Option("user", "User to get the guild avatar from")] DiscordUser? user = null)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(user is not null
			? ctx.GetGuildAvatarIfPossible(user)
			: ctx.GetGuildAvatarIfPossible(ctx.User)).Build()));

	[SlashCommand("server_info", "Get information about the server"), DeferResponseAsync(true)]
	public static async Task GetGuildInfoAsync(InteractionContext ctx)
	{
		if (ctx.Guild is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You have to execute this command on a server!"));
			return;
		}

		var members = await ctx.Guild.GetAllMembersAsync();
		var bots = members.Count(x => x.IsBot);
		var emb = new DiscordEmbedBuilder();
		emb.WithTitle(ctx.Guild.Name);
		if (ctx.Guild.BannerUrl is not null)
			emb.WithImageUrl(ctx.Guild.BannerUrl);
#pragma warning disable DCS0200
		if (ctx.Guild.Description is not null)
			emb.WithDescription(ctx.Guild.Description);
#pragma warning restore DCS0200
		emb.WithColor(new(0212255));
		if (ctx.Guild.IconUrl is not null)
			emb.WithThumbnail(ctx.Guild.IconUrl);
		emb.AddField(new("Owner", ctx.Guild.Owner?.UsernameWithGlobalName ?? "Unknown??".Italic()));
		emb.AddField(new("Language", ctx.Guild.PreferredLocale ?? "Not set".Italic()));
		emb.AddField(new("ID", ctx.Guild.Id.ToString()));
		emb.AddField(new("Created At", ctx.Guild.CreationTimestamp.Timestamp(TimestampFormat.LongDateTime)));
		emb.AddField(new("Members (Bots)", $"{members.Count} ({bots})"));
		emb.AddField(new("Emojis", ctx.Guild.Emojis.Count.ToString()));
		emb.AddField(new("Stickers", ctx.Guild.Stickers.Count.ToString()));
		emb.AddField(new("Soundboard Sounds", ctx.Guild.SoundboardSounds.Count.ToString()));
		emb.AddField(new("Roles", ctx.Guild.Roles.Count.ToString()));
		emb.AddField(new("Channels", ctx.Guild.Channels.Count.ToString()));
		emb.AddField(new("Scheduled Events", ctx.Guild.ScheduledEvents.Count.ToString()));
		emb.AddField(new("Threads", ctx.Guild.Threads.Count.ToString()));
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("user_info", "Get information about a user"), DeferResponseAsync(true)]
	public static async Task GetUserInfoAsync(InteractionContext ctx, [Option("user", "The user to view")] DiscordUser? user = null)
	{
		var member = user is not null && ctx.Guild is not null ? ctx.Guild.TryGetMember(user.Id, out var mem) ? mem : null : ctx.Member;
		user ??= ctx.User;
		var emb = new DiscordEmbedBuilder();
		emb.WithColor(new(0212255));
		emb.WithTitle("User Info");
		emb.AddField(new("Username", $"{user.UsernameWithGlobalName}"));
		if (member is not null)
			if (member.DisplayName != (user.GlobalName ?? user.Username))
				emb.AddField(new("Nickname", $"{member.DisplayName}"));
		emb.AddField(new("ID", $"{user.Id}"));
		emb.AddField(new("Account Creation", $"{user.CreationTimestamp.Timestamp()}"));
		if (member is not null)
			emb.AddField(new("Join Date", $"{member.JoinedAt.Timestamp()}"));
		emb.WithThumbnail(user.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("emojis", "Lists all custom emoji on this server"), DeferResponseAsync(true)]
	public static async Task ListEmojisAsync(InteractionContext ctx)
	{
		if (ctx.Guild is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You have to execute this command on a server!"));
			return;
		}

		if (ctx.Guild.Emojis.Count is 0)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("This server has no custom emojis!"));
			return;
		}

		var guildEmojis = ctx.Guild.Emojis.Values.ToList();
		var emojiGroups = guildEmojis.Select((emoji, index) => new
			{
				emoji,
				index
			})
			.GroupBy(x => x.index / 9)
			.Select(g => g.Select(x => x.emoji).ToList())
			.ToList();
		List<Page> pages = new(emojiGroups.Count);
		foreach (var group in emojiGroups)
		{
			DiscordEmbedBuilder builder = new();
			builder.WithTitle($"Emojis in {ctx.Guild.Name}");
			foreach (var emoji in group)
				builder.AddField(new(emoji.ToString(), $"{emoji.Name} ({emoji.Id})"));
			pages.Add(new(embed: builder));
		}

		await ctx.Client.GetInteractivity().SendPaginatedResponseAsync(ctx.Interaction, true, true, ctx.User, pages.Recalculate());
	}

	[SlashCommand("stickers", "Lists all custom stickers on this server"), DeferResponseAsync(true)]
	public static async Task ListStickersAsync(InteractionContext ctx)
	{
		if (ctx.Guild is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You have to execute this command on a server!"));
			return;
		}

		if (ctx.Guild.Stickers.Count is 0)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("This server has no custom stickers!"));
			return;
		}

		var guildStickers = ctx.Guild.Stickers.Values.ToList();
		List<Page> pages = new(guildStickers.Count);
		pages.AddRange(guildStickers.Select(guildSticker => new Page(embed: new DiscordEmbedBuilder().WithTitle($"Stickers in {ctx.Guild.Name}").AddField(new("Name", guildSticker.Name)).AddField(new("ID", guildSticker.Id.ToString())).AddField(new("Description", string.IsNullOrEmpty(guildSticker.Description) ? "No description".Italic() : guildSticker.Description)).WithImageUrl(guildSticker.Url))));
		await ctx.Client.GetInteractivity().SendPaginatedResponseAsync(ctx.Interaction, true, true, ctx.User, pages.Recalculate());
	}
}
