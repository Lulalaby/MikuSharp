namespace MikuSharp.Utilities;

/// <summary>
///     Provides fixed options for Discord commands.
/// </summary>
internal class FixedOptionProviders
{
	/// <summary>
	///     Provides choices for repeat modes.
	/// </summary>
	internal sealed class RepeatModeProvider : ChoiceProvider
	{
		/// <summary>
		///     Provides the choices for repeat modes.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result contains the choices for repeat modes.</returns>
		public override Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
		{
			var list = new List<DiscordApplicationCommandOptionChoice>(3)
			{
				new("None", $"{(int)RepeatMode.None}"),
				new("All", $"{(int)RepeatMode.All}"),
				new("Current", $"{(int)RepeatMode.Current}")
			};
			return Task.FromResult<IEnumerable<DiscordApplicationCommandOptionChoice>>(list);
		}
	}
}

/// <summary>
///     Provides autocomplete options for Discord commands.
/// </summary>
internal class AutocompleteProviders
{
	/// <summary>
	///     Provides autocomplete options for banned users.
	/// </summary>
	internal sealed class BanProvider : IAutocompleteProvider
	{
		/// <summary>
		///     Provides the autocomplete options for banned users.
		/// </summary>
		/// <param name="ctx">The autocomplete context.</param>
		/// <returns>
		///     A task that represents the asynchronous operation. The task result contains the autocomplete options for
		///     banned users.
		/// </returns>
		public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext ctx)
		{
			var bans = await ctx.Guild.GetBansAsync();
			List<DiscordBan> bannedUsers = new(25);
			bannedUsers.AddRange(ctx.FocusedOption.Value is not string value
				? bans.Take(25)
				: bans.Where(x => x.User.Username.ToLowerInvariant().Contains(Convert.ToString(value))).Take(25));

			return bannedUsers.Select(x => new DiscordApplicationCommandAutocompleteChoice(x.User.UsernameWithGlobalName, x.User.Id.ToString()));
		}
	}

	/// <summary>
	///     Provides autocomplete options for playlists.
	/// </summary>
	internal sealed class PlaylistProvider : IAutocompleteProvider
	{
		/// <summary>
		///     Provides the autocomplete options for playlists.
		/// </summary>
		/// <param name="ctx">The autocomplete context.</param>
		/// <returns>
		///     A task that represents the asynchronous operation. The task result contains the autocomplete options for
		///     playlists.
		/// </returns>
		public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext ctx)
			=> [];
	}

	/// <summary>
	///     Provides autocomplete options for songs.
	/// </summary>
	internal sealed class SongProvider : IAutocompleteProvider
	{
		/// <summary>
		///     Provides the autocomplete options for songs.
		/// </summary>
		/// <param name="ctx">The autocomplete context.</param>
		/// <returns>
		///     A task that represents the asynchronous operation. The task result contains the autocomplete options for
		///     songs.
		/// </returns>
		public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext ctx)
			=> [];
	}

	/// <summary>
	///     Provides autocomplete options for the queue.
	/// </summary>
	internal sealed class QueueProvider : IAutocompleteProvider
	{
		/// <summary>
		///     Provides the autocomplete options for the queue.
		/// </summary>
		/// <param name="ctx">The autocomplete context.</param>
		/// <returns>
		///     A task that represents the asynchronous operation. The task result contains the autocomplete options for the
		///     queue.
		/// </returns>
		public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext ctx)
		{
			return await ctx.ExecuteWithMusicSessionAsync((_, musicSession) =>
			{
				var value = ctx.FocusedOption.Value?.ToString();
				var queue = musicSession.LavalinkGuildPlayer?.Queue.ToList();
				if (queue is null || queue.Count is 0)
					return Task.FromResult<IEnumerable<DiscordApplicationCommandAutocompleteChoice>>([new("The queue is empty", -1)]);

				var queueEntries = queue
					.Select((entry, index) => (index: index + 1, entry))
					.ToDictionary(x => x.index, x => x.entry);
				var filteredQueueEntries = string.IsNullOrEmpty(value)
					? queueEntries.Take(25)
					: queueEntries.Where(x => x.Value.Info.Title.ToLowerInvariant().Contains(value.ToLower())).Take(25);
				return Task.FromResult(filteredQueueEntries.Select(x => new DiscordApplicationCommandAutocompleteChoice($"{x.Key}: {x.Value.Info.Title}", x.Key - 1)));
			}, null, [new("The queue is empty", -1)]);
		}
	}
}
