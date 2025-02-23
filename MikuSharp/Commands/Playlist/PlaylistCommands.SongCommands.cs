using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Playlist;

/// <summary>
///     Contains commands for managing songs in playlists.
/// </summary>
public partial class PlaylistCommands
{
	/// <summary>
	///     Provides commands for managing songs in playlists.
	/// </summary>
	[SlashCommandGroup("song", "Song management"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class SongCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Adds a song to a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to add the song to.</param>
		/// <param name="song">The URL or name of the song to add.</param>
		[SlashCommand("add", "Add a song to a playlist")]
		public static async Task AddSongAsync(
			InteractionContext ctx,
			[Option("playlist", "Name of playlist to add song to", true), Autocomplete(typeof(AutocompleteProviders.PlaylistProvider))]
			string playlist,
			[Option("url_or_name", "Url or name of song to add")]
			string song
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Add song")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Inserts a song into a playlist at a chosen position.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to add the song to.</param>
		/// <param name="posi">The position to insert the song at.</param>
		/// <param name="song">The URL or name of the song to add.</param>
		[SlashCommand("insert_at", "Insert a song into a playlist at a chosen position")]
		public static async Task InsertSongAtAsync(
			InteractionContext ctx,
			[Option("playlist", "Name of playlist to add song to", true), Autocomplete(typeof(AutocompleteProviders.PlaylistProvider))]
			string playlist,
			[Option("position", "Position to insert song at", true), Autocomplete(typeof(AutocompleteProviders.SongProvider))]
			string posi,
			[Option("url_or_name", "Url or name of song to add")]
			string song
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Insert song")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Moves a song to a specific position in a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to move the song within.</param>
		/// <param name="oldposi">The position to move the song from.</param>
		/// <param name="newposi">The position to move the song to.</param>
		[SlashCommand("move", "Move a song to a specific position in your playlist")]
		public static async Task MoveSongAsync(
			InteractionContext ctx,
			[Option("playlist", "Name of playlist to move the song within", true), Autocomplete(typeof(AutocompleteProviders.PlaylistProvider))]
			string playlist,
			[Option("old_position", "Position to move the song from", true), Autocomplete(typeof(AutocompleteProviders.SongProvider))]
			string oldposi,
			[Option("new_position", "Position to move song to", true), Autocomplete(typeof(AutocompleteProviders.SongProvider))]
			string newposi
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Move song")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Removes a song from a playlist.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="playlist">The name of the playlist to remove the song from.</param>
		/// <param name="posi">The song to remove.</param>
		[SlashCommand("remove", "Remove a song from a playlist")]
		public static async Task RemoveSongAsync(
			InteractionContext ctx,
			[Option("playlist", "Name of playlist to remove the song from", true), Autocomplete(typeof(AutocompleteProviders.PlaylistProvider))]
			string playlist,
			[Option("song", "Song to remove", true), Autocomplete(typeof(AutocompleteProviders.SongProvider))]
			string posi
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Remove song")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}
	}
}
