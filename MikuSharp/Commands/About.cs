﻿using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Enums;

using System.Linq;
using System.Threading.Tasks;

namespace MikuSharp.Commands;

[SlashCommandGroup("about", "About")]
internal class About : ApplicationCommandsModule
{
	[SlashCommand("donate", "Financial support information")]
	public static async Task DonateAsync(InteractionContext ctx)
	{
		var emb = new DiscordEmbedBuilder();
		emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl).
			WithTitle("Donate Page!").
			WithAuthor("Miku MikuBot uwu").
			WithUrl("https://meek.moe/").
			WithColor(new DiscordColor("#348573")).
			WithDescription("Thank you for your interest in supporting the bot's development!\n" +
			"Here are some links that may interest you").
			AddField(new DiscordEmbedField("Patreon", "[Link](https://patreon.com/speyd3r)", true)).
			AddField(new DiscordEmbedField("PayPal", "[Link](https://paypal.me/speyd3r)", true));
		await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(emb.Build()).AsEphemeral(ctx.Guild != null));
	}

	[SlashCommand("bot", "About the bot")]
	public static async Task BotAsync(InteractionContext ctx)
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null));
		var emb = new DiscordEmbedBuilder();
		emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl).
			WithTitle($"About {ctx.Client.CurrentUser.UsernameWithDiscriminator}!").
			WithAuthor("Miku MikuBot uwu").
			WithUrl("https://meek.moe/").
			WithColor(new DiscordColor("#348573")).
			WithDescription(ctx.Client.CurrentApplication.Description);
		foreach (var member in ctx.Client.CurrentApplication.Team.Members.OrderByDescending(x => x.User.Username))
			emb.AddField(new DiscordEmbedField(member.User.Id == ctx.Client.CurrentApplication.Team.Owner.Id ? "Owner" : "Developer", member.User.UsernameWithDiscriminator));
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("news", "Get news about the bot in your server", dmPermission: false)]
	public static async Task FollowNewsAsync(InteractionContext ctx, [Option("target_channel", "Target channel to post updates."), ChannelTypes(ChannelType.Text)] DiscordChannel channel, [Option("name", "Name of webhook")] string name = "Miku Bot Announcements")
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
		if (!ctx.Client.CurrentApplication.Team.Members.Where(x => x.User == ctx.User).Any() && ctx.User.Id != ctx.Guild.OwnerId)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You are not allowed to execute this request!"));
			return;
		}
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
		var memoryStream = new System.IO.MemoryStream();
		await stream.CopyToAsync(memoryStream);
		memoryStream.Position = 0;
		await webhook.ModifyAsync(name, memoryStream, reason: "Dev update follow");
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"News setup complete {DiscordEmoji.FromGuildEmote(client: MikuBot.ShardedClient.GetShard(483279257431441410), id: 623933340520546306)}\n\nYou'll get the newest news about the bot in your server in {channel.Mention}!"));
	}


	[SlashCommand("feedback", "Send feedback!")]
	public static async Task FeedbackAsync(InteractionContext ctx, [Option("feedback", "The feedback you want to send")] string feedback)
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null));
		var guild = await MikuBot.ShardedClient.GetShard(483279257431441410).GetGuildAsync(id: 483279257431441410);
		var emb = new DiscordEmbedBuilder();
		emb.WithAuthor(name: $"{ctx.User.UsernameWithDiscriminator}", iconUrl: ctx.User.AvatarUrl).
			WithTitle(title: "Feedback").
			WithDescription(feedback).
			WithFooter(text: $"Sent from {ctx.Guild?.Name ?? "DM"}");
		emb.AddField(new DiscordEmbedField(name: "User", value: $"{ctx.User.Mention}", inline: true));
		if (ctx.Guild != null)
			emb.AddField(new DiscordEmbedField(name: "Guild", value: $"{ctx.Guild.Id}", inline: true));
		var embed = await guild.GetChannel(484698873411928075).SendMessageAsync(embed: emb.Build());
		await embed.CreateReactionAsync(DiscordEmoji.FromName(client: ctx.Client, name: ":thumbsup:"));
		await embed.CreateReactionAsync(DiscordEmoji.FromName(client: ctx.Client, name: ":thumbsdown:"));
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Feedback sent {DiscordEmoji.FromGuildEmote(client: MikuBot.ShardedClient.GetShard(483279257431441410), id: 623933340520546306)}"));
	}

	[SlashCommand("ping", "Current ping to discord's services")]
	public static async Task PingAsync(InteractionContext ctx)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null).WithContent($"Ping: {ctx.Client.Ping}ms"));

	[SlashCommand("which_shard", "What shard am I on?")]
	public static async Task GetExecutingShardAsync(InteractionContext ctx)
		=> await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null).WithContent($"Shard {ctx.Client.ShardId}"));

	[SlashCommand("stats", "Some stats of the MikuBot!")]
	public static async Task StatsAsync(InteractionContext ctx)
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null));
		int GuildCount = 0;
		int UserCount = 0;
		int ChannelCount = 0;
		foreach (var client in MikuBot.ShardedClient.ShardClients)
		{
			GuildCount += client.Value.Guilds.Count;
			foreach (var guild in client.Value.Guilds)
			{
				UserCount += guild.Value.MemberCount;
				ChannelCount += guild.Value.Channels.Count;
			}
		}
		var emb = new DiscordEmbedBuilder().
			WithTitle("Stats").
			WithDescription("Some stats of the MikuBot!").
			AddField(new DiscordEmbedField("Guilds", GuildCount.ToString(), true)).
			AddField(new DiscordEmbedField("Users", UserCount.ToString(), true)).
			AddField(new DiscordEmbedField("Channels", ChannelCount.ToString(), true)).
			AddField(new DiscordEmbedField("Ping", ctx.Client.Ping.ToString(), true)).
			AddField(new DiscordEmbedField("Lib (Version)", ctx.Client.BotLibrary + " " + ctx.Client.VersionString, true)).
			WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()));
	}

	[SlashCommand("support", "Link to my support server")]
	public static async Task SupportAsybc(InteractionContext ctx)
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ctx.Guild != null));
		var guild = await MikuBot.ShardedClient.GetShard(483279257431441410).GetGuildAsync(id: 483279257431441410);
		var widget = await guild.GetWidgetAsync();
		var emb = new DiscordEmbedBuilder().
			WithTitle("Support Server").
			WithDescription("Need help or is something broken?").
			WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb.Build()).AddComponents(new DiscordLinkButtonComponent(widget.InstantInviteUrl, "Support Server", false, new DiscordComponentEmoji(704733597655105634))));
	}
}
