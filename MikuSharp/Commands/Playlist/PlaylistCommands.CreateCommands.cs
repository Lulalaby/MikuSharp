using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Playlist;

public partial class PlaylistCommands
{
	/// <summary>
	///     Provides commands for creating playlists.
	/// </summary>
	[SlashCommandGroup("create", "Playlist creation"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class CreateCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Copies the current queue to a new playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="name">The name of the new playlist.</param>
		[SlashCommand("from_queue", "Copy the current queue to a playlist!")]
		public static async Task CopyQueueToNewPlaylistAsync(InteractionContext ctx, [Option("name", "Name of new playlist")] string name)
		{
			await ctx.ExecuteWithMusicSessionAsync(
				async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist from queue")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o"))
			);
		}

		/// <summary>
		///     Creates a new blank playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="name">The name of the new playlist.</param>
		[SlashCommand("blank", "Create a new playlist from scratch")]
		public static async Task CreateBlankPlaylistAsync(InteractionContext ctx, [Option("name", "Name of new playlist")] string name)
		{
			await ctx.ExecuteWithMusicSessionAsync(
				async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist from scratch")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o"))
			);
		}

		/// <summary>
		///     Creates a new playlist from another playlist (e.g., YouTube, Soundcloud, or Spotify).
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="name">The name of the new playlist.</param>
		/// <param name="link">The link to the source playlist.</param>
		[SlashCommand("create", "Create a new playlist from another playlist (Like YouTube, Soundcloud or Spotify)")]
		public static async Task CreatePlaylistAsync(InteractionContext ctx, [Option("name", "Name of new playlist")] string name, [Option("source", "The playlist to use")] string link)
		{
			await ctx.ExecuteWithMusicSessionAsync(
				async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist from playlist")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o"))
			);
		}
	}
}
