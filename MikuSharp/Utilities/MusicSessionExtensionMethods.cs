using System.Diagnostics.CodeAnalysis;

using MikuSharp.Entities;
using MikuSharp.Enums;

namespace MikuSharp.Utilities;

/// <summary>
///     Provides extension methods for use with <see cref="MusicSession"/>s.
/// </summary>
internal static class MusicSessionExtensionMethods
{
	/// <summary>
	///     Gets the default session.
	/// </summary>
	/// <param name="lavalink">The lavalink extension.</param>
	/// <returns>The first session or <see langword="null" />.</returns>
	public static LavalinkSession? DefaultSession(this LavalinkExtension lavalink)
		=> lavalink.ConnectedSessions.Count > 0 ? lavalink.ConnectedSessions.First().Value : null;

	/// <summary>
	///     Builds a music status embed.
	/// </summary>
	/// <param name="session">The music session.</param>
	/// <param name="description">The description.</param>
	/// <param name="additionalEmbedFields">The additional embed fields.</param>
	/// <returns>The built embed.</returns>
	public static DiscordEmbed BuildMusicStatusEmbed(this MusicSession session, string description, List<DiscordEmbedField>? additionalEmbedFields = null)
	{
		var builder = new DiscordEmbedBuilder()
			.WithAuthor(MikuBot.ShardedClient.CurrentUser.UsernameWithGlobalName, iconUrl: MikuBot.ShardedClient.CurrentUser.AvatarUrl)
			.WithColor(DiscordColor.Black)
			.WithTitle("Miku Music Status")
			.WithDescription(description);

		builder.AddField(new("State", session.PlaybackState.ToString()));
		builder.AddField(new("Repeat Mode", session.RepeatMode.ToString()));

		if (additionalEmbedFields is null)
			return builder.Build();

		ArgumentOutOfRangeException.ThrowIfGreaterThan(additionalEmbedFields.Count, 23, nameof(additionalEmbedFields));
		builder.AddFields(additionalEmbedFields);

		return builder.Build();
	}

	/// <summary>
	///     Builds a music status embed.
	/// </summary>
	/// <param name="session">The music session.</param>
	/// <param name="additionalEmbedFields">The additional embed fields.</param>
	/// <returns>The built embed.</returns>
	public static DiscordEmbed BuildMusicStatusEmbed(this MusicSession session, List<DiscordEmbedField>? additionalEmbedFields = null)
		=> session.StatusMessage is not null ? BuildMusicStatusEmbed(session, session.StatusMessage.Embeds[0].Description, additionalEmbedFields) : throw new NullReferenceException();

	/// <summary>
	///     Loads and plays an <paramref name="identifier" />.
	/// </summary>
	/// <param name="musicSession">The music session.</param>
	/// <param name="ctx">The interaction context.</param>
	/// <param name="identifier">The identifier to load.</param>
	/// <param name="shufflePlaylists">Whether to shuffle playlists. Defaults to <see langword="false"/>.</param>
	/// <param name="searchType">The optional search type. Defaults to <see cref="LavalinkSearchType.Plain" />.</param>
	/// <returns>Whether the track was successfully loaded and added to the queue.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static async Task<MusicSession> LoadAndPlayTrackAsync(this MusicSession musicSession, InteractionContext ctx, string identifier, bool shufflePlaylists = false, LavalinkSearchType searchType = LavalinkSearchType.Plain)
	{
		var loadResult = await musicSession.LavalinkGuildPlayer.LoadTracksAsync(searchType, identifier);
		switch (loadResult.LoadType)
		{
			case LavalinkLoadResultType.Track:
				var track = loadResult.GetResultAs<LavalinkTrack>();
				musicSession.LavalinkGuildPlayer.AddToQueue(track);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added {track.Info.Title.Bold()} to the queue!"));
				break;
			case LavalinkLoadResultType.Playlist:
				var playlist = loadResult.GetResultAs<LavalinkPlaylist>();
				musicSession.LavalinkGuildPlayer.AddToQueue(playlist);
				if (shufflePlaylists)
					musicSession.LavalinkGuildPlayer.ShuffleQueue();
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added playlist {playlist.Info.Name.Bold()} to the queue."));
				break;
			case LavalinkLoadResultType.Search:
				var tracks = loadResult.GetResultAs<List<LavalinkTrack>>();
				musicSession.LavalinkGuildPlayer.AddToQueue(tracks.First());
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added {tracks.First().Info.Title.Bold()} to the queue!"));
				break;
			case LavalinkLoadResultType.Empty:
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"No results found for `{identifier.InlineCode()}`"));
				throw new("No results found");
			case LavalinkLoadResultType.Error:
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Something went wrong..\nReason: {loadResult.GetResultAs<LavalinkException>().Message ?? "unknown"}"));
				throw new("Lavalink error");
			default:
				throw new ArgumentOutOfRangeException(null, "Could not determine the type of the lavalink search result");
		}

		switch (musicSession.PlaybackState)
		{
			case PlaybackState.Stopped:
				musicSession.LavalinkGuildPlayer.PlayQueue();
				break;
			case PlaybackState.Paused:
				await musicSession.LavalinkGuildPlayer.ResumeAsync();
				musicSession.UpdatePlaybackState(PlaybackState.Playing);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed());
				break;
			case PlaybackState.Playing:
			default:
				break;
		}

		return musicSession;
	}

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <param name="ctx">The interaction context.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="finalAction">The action to execute after the success or failure action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public static async Task ExecuteWithMusicSessionAsync(this InteractionContext ctx, Func<ulong, MusicSession, Task> successAction, Func<ulong, Task>? failureAction = null, Func<ulong, Task>? finalAction = null)
	{
		ArgumentNullException.ThrowIfNull(ctx.GuildId);
		await ctx.GuildId.Value.ExecuteWithMusicSessionAsync(successAction, failureAction, finalAction);
	}

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <param name="ctx">The autocomplete context.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="finalAction">The action to execute after the success or failure action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public static async Task ExecuteWithMusicSessionAsync(this AutocompleteContext ctx, Func<ulong, MusicSession, Task> successAction, Func<ulong, Task>? failureAction = null, Func<ulong, Task>? finalAction = null)
		=> await ctx.Guild.Id.ExecuteWithMusicSessionAsync(successAction, failureAction, finalAction);

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <param name="guildId">The guild ID.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="finalAction">The action to execute after the success or failure action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public static async Task ExecuteWithMusicSessionAsync(this ulong guildId, Func<ulong, MusicSession, Task> successAction, Func<ulong, Task>? failureAction = null, Func<ulong, Task>? finalAction = null)
	{
		var asyncLock = MikuBot.MusicSessionLocks.GetOrAdd(guildId, _ => new());
		using (await asyncLock.LockAsync(MikuBot.Cts.Token))
		{
			if (MikuBot.MusicSessions.TryGetValue(guildId, out var musicSession))
				await successAction(guildId, musicSession);
			else if (failureAction is not null)
				await failureAction(guildId);
		}

		if (finalAction is not null)
			await finalAction(guildId);
	}

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	/// <param name="ctx">The interaction context.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="defaultValue">
	///     The default value of <typeparamref name="T" /> to return if the music session does not
	///     exist.
	/// </param>
	/// <returns>The result of the action or the default value with given type from <typeparamref name="T" />.</returns>
	public static async Task<T> ExecuteWithMusicSessionAsync<T>(this InteractionContext ctx, Func<ulong, MusicSession, Task<T>> successAction, Func<ulong, Task<T>>? failureAction = null, T defaultValue = default)
	{
		ArgumentNullException.ThrowIfNull(ctx.GuildId);
		return await ctx.GuildId.Value.ExecuteWithMusicSessionAsync(successAction, failureAction, defaultValue);
	}

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	/// <param name="ctx">The autocomplete context.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="defaultValue">
	///     The default value of <typeparamref name="T" /> to return if the music session does not
	///     exist.
	/// </param>
	/// <returns>The result of the action or the default value with given type from <typeparamref name="T" />.</returns>
	public static async Task<T> ExecuteWithMusicSessionAsync<T>(this AutocompleteContext ctx, Func<ulong, MusicSession, Task<T>> successAction, Func<ulong, Task<T>>? failureAction = null, T defaultValue = default)
		=> await ctx.Guild.Id.ExecuteWithMusicSessionAsync(successAction, failureAction, defaultValue);

	/// <summary>
	///     Executes an action with the music session.
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	/// <param name="guildId">The guild ID.</param>
	/// <param name="successAction">The action to execute.</param>
	/// <param name="failureAction">The action to execute if the music session does not exist.</param>
	/// <param name="defaultValue">
	///     The default value of <typeparamref name="T" /> to return if the music session does not
	///     exist.
	/// </param>
	/// <returns>The result of the action or the default value with given type from <typeparamref name="T" />.</returns>
	public static async Task<T> ExecuteWithMusicSessionAsync<T>(this ulong guildId, Func<ulong, MusicSession, Task<T>> successAction, Func<ulong, Task<T>>? failureAction = null, T defaultValue = default)
	{
		var asyncLock = MikuBot.MusicSessionLocks.GetOrAdd(guildId, _ => new());
		using (await asyncLock.LockAsync(MikuBot.Cts.Token))
		{
			if (MikuBot.MusicSessions.TryGetValue(guildId, out var musicSession))
				return await successAction(guildId, musicSession);
			if (failureAction is not null)
				return await failureAction(guildId);
		}

		return defaultValue;
	}
}
