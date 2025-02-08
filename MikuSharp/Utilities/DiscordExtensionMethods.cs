using System.Diagnostics.CodeAnalysis;

using HeyRed.Mime;

using MikuSharp.Entities;

using Weeb.net.Data;

namespace MikuSharp.Utilities;

/// <summary>
///     Contains extension methods for Discord-related classes.
/// </summary>
public static class DiscordExtensionMethods
{
	/// <summary>
	///     Gets the avatar URL of the user, using the guild avatar URL if possible.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <param name="user">The user.</param>
	/// <returns>The avatar URL.</returns>
	public static string GetGuildAvatarIfPossible(this BaseContext ctx, DiscordUser user)
		=> ctx.Guild is not null && ctx.Guild.TryGetMember(user.Id, out var member)
			? member.GuildAvatarUrl
			: user.AvatarUrl;

	/// <summary>
	///     Gets the display name of the user, using the guild display name if possible.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <param name="user">The user.</param>
	/// <returns>The display name.</returns>
	public static string GetGuildOrGlobalDisplayNameIfPossible(this BaseContext ctx, DiscordUser user)
		=> ctx.Guild is not null && ctx.Guild.TryGetMember(user.Id, out var member)
			? member.DisplayName
			: user.GetGlobalOrUsername();

	/// <summary>
	///     Gets the global name of the user, using the username if the global name is not set.
	/// </summary>
	/// <param name="user">The user.</param>
	/// <returns>The global name or username.</returns>
	public static string GetGlobalOrUsername(this DiscordUser user)
		=> user.GlobalName ?? user.Username;

	/// <summary>
	///     Tries to build and send a components V2 action message.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="image">The image.</param>
	/// <param name="title">The optional title.</param>
	/// <param name="content">The optional content.</param>
	/// <param name="user">The additional user to allow to be pinged (author is already added).</param>
	/// <param name="footer">
	///     The optional footer. Tho footer is not really the correct word for the new components. We use a
	///     section instead.
	/// </param>
	/// <returns>Whether the message was sent successfully.</returns>
	public static async Task<bool> TryBuildV2ActionMessageAsync(this BaseContext context, MemoryStream image, string? title = null, string? content = null, DiscordUser? user = null, string? footer = null)
	{
		if (context.GuildId is not 1317206872763404478)
			return false;

		DiscordWebhookBuilder builder = new();
		builder.WithV2Components();
		builder.AddFile($"image.{MimeGuesser.GuessExtension(image)}", image);
		DiscordContainerComponent container = new();
		if (title is not null)
			container.AddComponent(new DiscordTextDisplayComponent(title));
		if (content is not null)
			container.AddComponent(new DiscordTextDisplayComponent(content));
		container.AddComponent(new DiscordMediaGalleryComponent([new($"attachment://image.{MimeGuesser.GuessExtension(image)}")]));
		if (footer is not null)
			container.AddComponent(new DiscordTextDisplayComponent(footer.Subtext()));
		builder.AddComponents(container);
		builder.WithAllowedMention(new UserMention(context.User));
		if (user is not null)
			builder.WithAllowedMention(new UserMention(user));
		await context.EditResponseAsync(builder);
		await image.DisposeAsync();
		return true;
	}

	/// <summary>
	///     Sends an old-style embed message.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="image">The image.</param>
	/// <param name="content">The optional content.</param>
	/// <param name="user">The additional user to allow to be pinged (author is already added).</param>
	/// <param name="footer">The optional footer.</param>
	public static async Task SendOldStyleMessageAsync(this BaseContext context, MemoryStream image, string? content = null, DiscordUser? user = null, string? footer = null)
	{
		DiscordWebhookBuilder builder = new();
		var em = new DiscordEmbedBuilder();
		if (content is not null)
			em.WithDescription(content);
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(image)}");
		if (footer is not null)
			em.WithFooter(footer);
		builder.AddFile($"image.{MimeGuesser.GuessExtension(image)}", image);
		builder.AddEmbed(em.Build());
		if (user is not null)
			builder.WithContent(user.Mention);
		builder.WithAllowedMention(new UserMention(context.User));
		if (user is not null)
			builder.WithAllowedMention(new UserMention(user));
		await context.EditResponseAsync(builder);
		await image.DisposeAsync();
	}

	/// <summary>
	///     Tries to get an image from the Weeb.net API.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="data">The data.</param>
	/// <param name="stream">The stream.</param>
	/// <returns>Whether the image was successfully retrieved.</returns>
	public static bool TryGetWeebNetImage(this BaseContext context, RandomData? data, [NotNullWhen(true)] out MemoryStream? stream)
	{
		if (data is null)
		{
			stream = null;
			return false;
		}

		stream = new(context.Client.RestClient.GetByteArrayAsync(data.Url.ResizeLink()).Result);
		return true;
	}

	/// <summary>
	///     Tries to get an image from the Weeb.sh API.
	/// </summary>
	/// <param name="data">The data.</param>
	/// <param name="stream">The stream.</param>
	/// <returns>Whether the image was successfully retrieved.</returns>
	public static bool TryGetWeebShImage(this WeebSh? data, [NotNullWhen(true)] out MemoryStream? stream)
	{
		if (data is null)
		{
			stream = null;
			return false;
		}

		stream = data.ImgData;
		return true;
	}

	/// <summary>
	///     Responds with an error message.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <param name="content">The content.</param>
	/// <param name="user">The user.</param>
	public static async Task ActionRespondWithErrorAsync(this BaseContext ctx, string content, DiscordUser user)
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(content).WithAllowedMentions([new UserMention(ctx.User), new UserMention(user)]));
		await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent("Failed to get image"));
	}

	/// <summary>
	///    Responds with an error message if the <paramref name="imgData"/> is null.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <param name="imgData">The image data to check.</param>
	/// <returns>Whether the image data is not null.</returns>
	public static async Task<bool> CheckForProperImageResultAsync(this BaseContext ctx, [NotNullWhen(true)] ImgData? imgData)
	{
		if (imgData is not null)
			return true;

		await ctx.FollowUpAsync("Something went wrong while fetching the image");
		return false;
	}
}
