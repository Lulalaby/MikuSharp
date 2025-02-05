using HeyRed.Mime;

using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("action", "Actions", allowedContexts: [InteractionContextType.Guild, InteractionContextType.PrivateChannel], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall, ApplicationCommandIntegrationTypes.UserInstall]), DeferResponseAsync]
internal class Action : ApplicationCommandsModule
{
	[SlashCommand("hug", "Hug someone!")]
	public static async Task HugAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var wsh = await ctx.Client.RestClient.GetWeebShAsync("hug");
		if (wsh is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} hugs {user.Mention} uwu").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## A wild hug appears!"), new DiscordTextDisplayComponent($"{ctx.User.Mention} hugs {user.Mention} uwu"), new DiscordMediaGalleryComponent([new($"attachment://image.{wsh.Extension}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		wsh.Embed.WithDescription($"{ctx.User.Mention} hugs {user.Mention} uwu");
		builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
		builder.AddEmbed(wsh.Embed.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("kiss", "Kiss someone!")]
	public static async Task KissAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var wsh = await ctx.Client.RestClient.GetWeebShAsync("kiss");
		if (wsh is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} kisses {user.Mention} >~<").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## A kiss!"), new DiscordTextDisplayComponent($"{ctx.User.Mention} kisses {user.Mention} >~<"), new DiscordMediaGalleryComponent([new($"attachment://image.{wsh.Extension}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		wsh.Embed.WithDescription($"{ctx.User.Mention} kisses {user.Mention} >~<");
		builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
		builder.AddEmbed(wsh.Embed.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("lick", "Lick someone!")]
	public static async Task LickAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var wsh = await ctx.Client.RestClient.GetWeebShAsync("lick");
		if (wsh is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} licks {user.Mention} owo").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Slurp~"), new DiscordTextDisplayComponent($"{ctx.User.Mention} licks {user.Mention} owo"), new DiscordMediaGalleryComponent([new($"attachment://image.{wsh.Extension}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		wsh.Embed.WithDescription($"{ctx.User.Mention} licks {user.Mention} owo");
		builder.AddFile($"image.{wsh.Extension}", wsh.ImgData);
		builder.AddEmbed(wsh.Embed.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("pat", "Pat someone!")]
	public static async Task PatAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("pat", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} pats {user.Mention} #w#").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Pat pat :3"), new DiscordTextDisplayComponent($"{ctx.User.Mention} pats {user.Mention} #w#"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} pats {user.Mention} #w#");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("poke", "Poke someone!")]
	public static async Task PokeAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("poke", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} pokes {user.Mention} ÓwÒ").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Poke poke :p"), new DiscordTextDisplayComponent($"{ctx.User.Mention} pokes {user.Mention} ÓwÒ"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} pokes {user.Mention} ÓwÒ");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("slap", "Slap someone!")]
	public static async Task SlapAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("slap", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} slaps {user.Mention} ÒwÓ").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Slap x~x"), new DiscordTextDisplayComponent($"{ctx.User.Mention} slaps {user.Mention} ÒwÓ"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} slaps {user.Mention} ÒwÓ");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("bite", "Bite someone!")]
	public static async Task BiteAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("bite", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} bites {user.Mention} x~x").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Bite~"), new DiscordTextDisplayComponent($"{ctx.User.Mention} bites {user.Mention} x~x"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} bites {user.Mention} x~x");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("nom", "Nom someone!")]
	public static async Task NomAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("nom", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} noms {user.Mention} >:3c").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Noms~"), new DiscordTextDisplayComponent($"{ctx.User.Mention} noms {user.Mention} >:3c"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} noms {user.Mention} >:3c");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}

	[SlashCommand("stare", "Stare at someone!")]
	public static async Task StateAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var weeurl = await MikuBot.WeebClient.GetRandomAsync("stare", []);
		if (weeurl is null)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} stares at {user.Mention} O.o").WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
			await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
			return;
		}

		DiscordWebhookBuilder builder = new();
		var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(weeurl.Url.ResizeLink()));

		if (ctx.GuildId is 1317206872763404478)
		{
			builder.WithV2Components();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## Stare O.o"), new DiscordTextDisplayComponent($"{ctx.User.Mention} stares at {user.Mention} O.o"), new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(img)}")])]));
			builder.WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]);
			await ctx.EditResponseAsync(builder);
			return;
		}

		var em = new DiscordEmbedBuilder();
		em.WithDescription($"{ctx.User.Mention} stares at {user.Mention} O.o");
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by nekos.life");
		builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
		builder.AddEmbed(em.Build());
		builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(user));
		await ctx.EditResponseAsync(builder);
	}
}
