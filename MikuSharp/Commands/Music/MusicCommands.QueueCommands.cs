using MikuSharp.Attributes;
using MikuSharp.Enums;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Music;

public partial class MusicCommands
{
	/// <summary>
	///     The queue commands.
	/// </summary>
	[SlashCommandGroup("queue", "Music queue commands"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class QueueCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Shows the current queue.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("show", "Show the current queue")]
		public static async Task ShowQueueAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Queue list")); });
		}

		/// <summary>
		///     Skips to the next song.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("skip", "Skips to the next song")]
		public static async Task SkipAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (discard, musicSession) =>
			{
				if (musicSession.LavalinkGuildPlayer!.TryPeekQueue(out _))
				{
					await musicSession.LavalinkGuildPlayer.SkipAsync();
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully skipped the song!"));
				}
				else
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Cannot skip as there are no more songs in the queue."));
			});
		}

		/// <summary>
		///     Skips to the given queue position.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="position">The position in the queue.</param>
		[SlashCommand("skip_to", "Skips to given queue position")]
		public static async Task SkipToAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.QueueProvider)), Option("position", "Position in queue", true)] int position)
		{
			if (position is -1)
			{
				await ctx.EditResponseAsync("Something went wrong while parsing the queue position.");
				return;
			}

			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.LavalinkGuildPlayer!.RemoveFromQueueAtRange(0, position);
				await musicSession.LavalinkGuildPlayer.SkipAsync();
				if (musicSession.LavalinkGuildPlayer.TryPeekQueue(out var nextTrack))
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully skipped to {nextTrack.Info.Title.Bold()}!"));
				else
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Cannot skip as there are no more songs in the queue."));
			});
		}

		/// <summary>
		///     Removes a song from the queue.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="position">The position in the queue.</param>
		[SlashCommand("remove", "Remvoes a song from the queue")]
		public static async Task RemoveAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.QueueProvider)), Option("song", "Song to remove", true)] int position)
		{
			if (position is -1)
			{
				await ctx.EditResponseAsync("Something went wrong while parsing the song.");
				return;
			}

			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.LavalinkGuildPlayer!.RemoveFromQueueAt(position);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully removed the song from the queue!"));
			});
		}

		/// <summary>
		///     Clears the queue.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("clear", "Clear Queue"), RequirePlaybackState(PlaybackState.Playing, PlaybackState.Paused)]
		public static async Task StopAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.UpdateRepeatMode(RepeatMode.None);
				musicSession.LavalinkGuildPlayer.ClearQueue();
				await ctx.EditResponseAsync("Cleared the queue!");
			});
		}
	}
}
