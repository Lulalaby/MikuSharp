using MikuSharp.Attributes;
using MikuSharp.Enums;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Music;

public partial class MusicCommands
{
	/// <summary>
	///     The playback commands.
	/// </summary>
	[SlashCommandGroup("playback", "Music playback commands"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class PlaybackCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Pauses the playback.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("pause", "Pauses the playback"), RequirePlaybackState(PlaybackState.Playing)]
		public async Task PauseAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				await musicSession.LavalinkGuildPlayer.PauseAsync();
				musicSession.UpdatePlaybackState(PlaybackState.Paused);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed());
				await ctx.EditResponseAsync("Paused the playback! ");
			});
		}

		/// <summary>
		///     Resumes the playback.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("resume", "Resumes the playback"), RequirePlaybackState(PlaybackState.Paused)]
		public async Task ResumeAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				await musicSession.LavalinkGuildPlayer.ResumeAsync();
				musicSession.UpdatePlaybackState(PlaybackState.Playing);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed());
				await ctx.EditResponseAsync("Resumed the playback!");
			});
		}

		/// <summary>
		///     Stops the playback.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("stop", "Stop Playback"), RequirePlaybackState(PlaybackState.Playing, PlaybackState.Paused)]
		public static async Task StopAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.UpdateRepeatMode(RepeatMode.None);
				musicSession.LavalinkGuildPlayer.ClearQueue();
				await musicSession.LavalinkGuildPlayer.StopAsync();
				musicSession.UpdatePlaybackState(PlaybackState.Stopped);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed("Nothing playing"));
				await ctx.EditResponseAsync("Stopped the playback!");
			});
		}

		/// <summary>
		///     Seeks the currently playing song to given position.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="seconds">The seconds to seek to.</param>
		[SlashCommand("seek", "Seeks the currently playing song to given position"), RequirePlaybackState(PlaybackState.Playing, PlaybackState.Paused)]
		public static async Task SeekAsync(InteractionContext ctx, [Option("seconds", "Seconds to seek to")] int seconds)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				var targetSeek = TimeSpan.FromSeconds(seconds);
				await musicSession.LavalinkGuildPlayer.SeekAsync(seconds);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Seeked to **{targetSeek.FormatTimeSpan()}**!"));
			});
		}

		/// <summary>
		///     Plays a url.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="url">The url to play.</param>
		/// <param name="shufflePlaylists">Whether to shuffle playlists.</param>
		[SlashCommand("play", "Plays a url")]
		public async Task PlayUrlAsync(InteractionContext ctx, [Option("url", "The url to play")] string url, [Option("shuffle_playlist", "Shuffle playlists")] bool shufflePlaylists = false)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Searching for `{url}`.."));
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => await musicSession.LoadAndPlayTrackAsync(ctx, url, shufflePlaylists));
		}
	}
}
