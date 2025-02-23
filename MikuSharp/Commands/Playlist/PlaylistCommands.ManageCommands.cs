using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Playlist;

public partial class PlaylistCommands
{
	/// <summary>
	///     Provides commands for managing playlists.
	/// </summary>
	[SlashCommandGroup("manage", "Playlist management"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class ManageCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Lists all your playlists.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("list", "List all your playlists")]
		public static async Task ListPlaylistsAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlists")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Shows the contents of a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to show.</param>
		[SlashCommand("show", "Show the contents of a playlist")]
		public static async Task ShowPlaylistAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.PlaylistProvider)), Option("playlist", "Name of playlist to show", true)] string playlist)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Deletes a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to delete.</param>
		[SlashCommand("delete", "Delete a playlist")]
		public static async Task DeletePlaylistAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.PlaylistProvider)), Option("playlist", "Name of playlist to delete", true)] string playlist)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist delete")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Renames a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to rename.</param>
		/// <param name="name">The new name for the playlist.</param>
		[SlashCommand("rename", "Rename a playlist")]
		public static async Task RenamePlaylistAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.PlaylistProvider)), Option("playlist", "Name of playlist to rename", true)] string playlist, [Option("name", "New name for playlist")] string name)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist rename")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Clears all entries from a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to clear.</param>
		[SlashCommand("clear", "Clear all entries from a playlist")]
		public static async Task ClearPlaylistAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.PlaylistProvider)), Option("playlist", "Name of playlist to clear", true)] string playlist)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist clear")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Plays a playlist or adds the songs to the queue.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to play.</param>
		[SlashCommand("play", "Play a playlist/Add the songs to the queue")]
		public static async Task PlayPlaylistAsync(InteractionContext ctx, [Autocomplete(typeof(AutocompleteProviders.PlaylistProvider)), Option("playlist", "Name of playlist to play", true)] string playlist)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playlist play")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}
	}
}
