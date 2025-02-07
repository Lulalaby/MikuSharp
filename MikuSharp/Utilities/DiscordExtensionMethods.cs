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
	///    Gets the global name of the user, using the username if the global name is not set.
	/// </summary>
	/// <param name="user">The user.</param>
	/// <returns>The global name or username.</returns>
	public static string GetGlobalOrUsername(this DiscordUser user)
		=> user.GlobalName ?? user.Username;
}
