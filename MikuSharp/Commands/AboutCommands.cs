using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("about", "About")]
internal class AboutCommands : ApplicationCommandsModule
{
	[SlashCommand("donate", "Financial support information")]
	public static async Task DonateAsync(InteractionContext ctx)
	{
		var emb = new DiscordEmbedBuilder();
		emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl).WithTitle("Donate Page!").WithAuthor("Miku MikuBot uwu").WithColor(new("#348573"))
			.WithDescription("Thank you for your interest in supporting the bot's development!\n" + "Here are some links that may interest you").AddField(new("Patreon (Owner)", "[sekoree](https://patreon.com/sekoree)"))
			.AddField(new("PayPal (Owner)", "[speyd3r](https://paypal.me/speyd3r)")).AddField(new("PayPal (Current Developer)", "[aitsys](https://paypal.me/aitsys)")).AddField(new("GitHub Sponsers (Current Developer)", "[Lulalaby](https://github.com/sponsors/Lulalaby)"));
		await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(emb.Build()).AsEphemeral());
	}

	[SlashCommand("bot", "Information about the bot"), DeferResponseAsync(true)]
	public static async Task BotAsync(InteractionContext ctx)
	{
		var emb = new DiscordEmbedBuilder();
		emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl).WithTitle($"About {ctx.Client.CurrentApplication.Name}!").WithAuthor("Miku MikuBot uwu").WithColor(new("#348573"));
		if (ctx.Client.CurrentApplication.Description is not null)
			emb.WithDescription(ctx.Client.CurrentApplication.Description);
		if (ctx.Client.CurrentApplication.Team is not null)
			foreach (var member in ctx.Client.CurrentApplication.Team.Members.OrderByDescending(x => x.User.Username))
				emb.AddField(new(member.User.Id == ctx.Client.CurrentApplication.Team.Owner.Id
					? "Owner"
					: "Developer", member.User.UsernameWithGlobalName));
		else
			emb.AddField(new("Owner", ctx.Client.CurrentApplication.Owner.UsernameWithGlobalName));
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("news", "Get news about the bot in your server", allowedContexts: [InteractionContextType.Guild], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall]), DeferResponseAsync(true)]
	public static async Task FollowNewsAsync(
		InteractionContext ctx,
		[Option("target_channel", "Target channel to post updates."), ChannelTypes(ChannelType.Text)]
		DiscordChannel channel,
		[Option("name", "Name of webhook")] string name = "Miku Bot Announcements"
	)
	{
		var announcementChannel = await ctx.Client.GetChannelAsync(483290389047017482);
		var f = await announcementChannel.FollowAsync(channel);
		await Task.Delay(5000);
		var msgs = await channel.GetMessagesAsync();
		var target = msgs.First(x => x.MessageType == MessageType.ChannelFollowAdd);
		await target.DeleteAsync("Message cleanup");
		var webhooks = await channel.GetWebhooksAsync();
		var webhook = webhooks.First(x => x.Id == f.WebhookId);
		var selfAvatarUrl = ctx.Client.CurrentUser.AvatarUrl;
		var stream = await ctx.Client.RestClient.GetStreamAsync(selfAvatarUrl);
		var memoryStream = new MemoryStream();
		await stream.CopyToAsync(memoryStream);
		memoryStream.Position = 0;
		await webhook.ModifyAsync(name, memoryStream, reason: "Dev update follow");
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
			$"News setup complete {DiscordEmoji.FromGuildEmote(HatsuneMikuBot.ShardedClient.GetShard(483279257431441410), 623933340520546306)}\n\nYou'll get the newest news about the bot in your server in {channel.Mention}!"));
	}

	[SlashCommand("feedback", "Send feedback to the developers")]
	public static async Task FeedbackAsync(InteractionContext ctx)
	{
		DiscordInteractionModalBuilder modalBuilder = new();
		modalBuilder.WithTitle("Feedback modal");
		modalBuilder.AddTextComponent(new(TextComponentStyle.Small, "Title of feedback", "feedbacktitle", null, 5, null, true, "Feedback"));
		modalBuilder.AddTextComponent(new(TextComponentStyle.Paragraph, "Your feedback", "feedbackbody", null, 20));
		await ctx.CreateModalResponseAsync(modalBuilder);

		var res = await ctx.Client.GetInteractivity().WaitForModalAsync(modalBuilder.CustomId, TimeSpan.FromMinutes(2));

		if (!res.TimedOut)
		{
			await res.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
			var title = res.Result.Interaction.Data.Components.First(x => x.CustomId is "feedbacktitle").Value;
			var body = res.Result.Interaction.Data.Components.First(x => x.CustomId is "feedbackbody").Value;
			var guild = await HatsuneMikuBot.ShardedClient.GetShard(483279257431441410).GetGuildAsync(483279257431441410);
			var emb = new DiscordEmbedBuilder();
			emb.WithAuthor($"{ctx.User.UsernameWithGlobalName}", iconUrl: ctx.User.AvatarUrl).WithTitle(title).WithDescription(body);
			if (ctx.Guild is not null)
				emb.AddField(new("Guild", $"{ctx.Guild.Id}", true));
			var forum = await ctx.Client.GetChannelAsync(1020433162662322257, true);
			List<ForumPostTag> tags =
			[
				ctx.Guild is not null
					? forum.AvailableTags.First(x => x.Id is 1020434799493648404)
					: forum.AvailableTags.First(x => x.Id is 1020434935502360576)
			];
			var thread = await forum.CreatePostAsync("Feedback", new DiscordMessageBuilder().AddEmbed(emb.Build()).WithContent($"Feedback from {ctx.User.UsernameWithGlobalName}"), null, tags, "Feedback");
			var msg = await thread.GetMessageAsync(thread.Id);
			await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
			await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
			await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"Feedback sent {DiscordEmoji.FromGuildEmote(HatsuneMikuBot.ShardedClient.GetShard(483279257431441410), 623933340520546306)}"));
		}
		else
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("You were too slow :(\nThe time limit is two minutes.").AsEphemeral());
	}

	[SlashCommand("ping", "Current ping to discord's services")]
	public static async Task PingAsync(InteractionContext ctx)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"Ping: {$"{ctx.Client.Ping}ms".InlineCode()}"));

	[SlashCommand("which_shard", "Gets the id of current shard you're using me on")]
	public static async Task GetExecutingShardAsync(InteractionContext ctx)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"Shard: {ctx.Client.ShardId.ToString().InlineCode()}"));

	[SlashCommand("stats", "Statistics about the bot!"), DeferResponseAsync(true)]
	public static async Task StatsAsync(InteractionContext ctx)
	{
		var statistics = HatsuneMikuBot.ShardedClient.Statistics;

		var knownGuildFeatures = HatsuneMikuBot.ShardedClient.ShardClients.Values.SelectMany(client => client.Guilds.Values).SelectMany(guild => guild.RawFeatures ?? ["None"]).Distinct().ToList();

		var averagePing = (int)HatsuneMikuBot.ShardedClient.ShardClients.Values.Average(client => client.Ping);

		DiscordEmbedBuilder builder = new();
		builder.WithTitle("Stats");
		builder.WithDescription($"Some stats about {ctx.Client.CurrentApplication.Name}!\n\nKnown Guild Features:\n{string.Join("\n", knownGuildFeatures.Where(feature => feature is not "None")).BlockCode()}");
		foreach (var (key, value) in statistics)
			builder.AddField(new(key.ToString(), value.ToString().InlineCode(), true));
		builder.AddField(new("Ping", $"{averagePing}ms".InlineCode(), true));
		if (ctx.Client.VersionString.Contains('+'))
			builder.AddField(new("Lib (Version)", $"{ctx.Client.BotLibrary}@{ctx.Client.VersionString}".MaskedUrl(new($"https://github.com/Aiko-IT-Systems/DisCatSharp/tree/{ctx.Client.VersionString.Split('+').Last()}")), true));
		else
			builder.AddField(new("Lib (Version)", $"{ctx.Client.BotLibrary}@{ctx.Client.VersionString}".MaskedUrl(new($"https://github.com/Aiko-IT-Systems/DisCatSharp/tree/v{ctx.Client.VersionString.Trim()}")), true));
		builder.AddField(new("API Channel (Discord)", ctx.Client.ApiChannel.ToString().InlineCode(), true));
		builder.AddField(new("API Version (Discord)", ctx.Client.ApiVersion.InlineCode(), true));
		var lavalinkDefaultSession = ctx.Client.GetLavalink()?.DefaultSession();
		if (lavalinkDefaultSession is not null)
			builder.AddField(new("Lavalink Version", $"{await lavalinkDefaultSession.GetLavalinkVersionAsync()}".InlineCode(), true));
		builder.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
		if (ctx.Client.CurrentUser.BannerUrl is not null)
			builder.WithImageUrl(ctx.Client.CurrentUser.BannerUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(builder.Build()));
	}

	[SlashCommand("support", "Link to the support server"), DeferResponseAsync(true)]
	public static async Task SupportAsybc(InteractionContext ctx)
	{
		var guild = await HatsuneMikuBot.ShardedClient.GetShard(483279257431441410).GetGuildAsync(483279257431441410);
		var widget = await guild.GetWidgetAsync();
		var emb = new DiscordEmbedBuilder().WithTitle("Support Server").WithDescription("Need help or is something broken?").WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()).AddComponents(new DiscordLinkButtonComponent(widget.InstantInviteUrl, "Support Server", false, new(704733597655105634))));
	}
}
