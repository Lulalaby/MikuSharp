using DisCatSharp.Exceptions;

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
	public static async Task GuildInfoAsync(InteractionContext ctx)
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
		emb.WithColor(new(0212255));
		if (ctx.Guild.IconUrl is not null)
			emb.WithThumbnail(ctx.Guild.IconUrl);
		emb.AddField(new("Owner", ctx.Guild.Owner?.UsernameWithGlobalName ?? "unknown??".Italic(), true));
		emb.AddField(new("Language", ctx.Guild.PreferredLocale ?? "Not set".Italic(), true));
		emb.AddField(new("ID", ctx.Guild.Id.ToString(), true));
		emb.AddField(new("Created At", ctx.Guild.CreationTimestamp.Timestamp(TimestampFormat.LongDateTime), true));
		emb.AddField(new("Emojis", ctx.Guild.Emojis.Count.ToString(), true));
		emb.AddField(new("Members (Bots)", $"{members.Count} ({bots})", true));

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("user_info", "Get information about a user"), DeferResponseAsync(true)]
	public static async Task UserInfoAsync(InteractionContext ctx, [Option("user", "The user to view")] DiscordUser? user = null)
	{
		user ??= ctx.User;

		DiscordMember? member = null;

		if (ctx.Guild is not null)
			try
			{
				member = await user.ConvertToMember(ctx.Guild);
			}
			catch (NotFoundException)
			{ }

		var emb = new DiscordEmbedBuilder();
		emb.WithColor(new(0212255));
		emb.WithTitle("User Info");
		emb.AddField(new("Username", $"{user.UsernameWithGlobalName}", true));
		if (member is not null)
			if (member.DisplayName != user.Username)
				emb.AddField(new("Nickname", $"{member.DisplayName}", true));
		emb.AddField(new("ID", $"{user.Id}", true));
		emb.AddField(new("Account Creation", $"{user.CreationTimestamp.Timestamp()}", true));
		if (member is not null)
			emb.AddField(new("Join Date", $"{member.JoinedAt.Timestamp()}", true));
		emb.WithThumbnail(user.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("emojilist", "Lists all custom emoji on this server")]
	public static async Task EmojiListAsync(InteractionContext ctx)
	{
		var wat = "You have to execute this command in a server!";

		if (ctx.Guild is not null && ctx.Guild.Emojis.Any())
			wat = ctx.Guild.Emojis.Values.Aggregate("**Emojies:** ", (current, em) => current + em + " ");

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(wat));
	}
}
