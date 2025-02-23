namespace MikuSharp.Utilities;

/// <summary>
///     Represents a collection of formatters.
/// </summary>
internal static class Formatters
{
	/// <summary>
	///     Resizes an image link.
	/// </summary>
	/// <param name="url">The url of the image to resize.</param>
	/// <returns>The resized image.</returns>
	public static string ResizeLink(this string url)
		=> $"https://api.meek.moe/im/?image={url}&resize=500";

	/// <summary>
	///     Formats a <see cref="TimeSpan" /> into a human-readable string.
	/// </summary>
	/// <param name="timeSpan">The time span to format.</param>
	/// <returns>The formatted time span.</returns>
	public static string FormatTimeSpan(this TimeSpan timeSpan)
		=> timeSpan.TotalHours >= 1
			? $"{(int)timeSpan.TotalHours:D2}h:{timeSpan.Minutes:D2}m:{timeSpan.Seconds:D2}s"
			: timeSpan.TotalMinutes >= 1
				? $"{(int)timeSpan.TotalMinutes:D2}m:{timeSpan.Seconds:D2}s"
				: $"{(int)timeSpan.TotalSeconds:D2}s";

	/// <summary>
	///    Maps a source name to an emoji.
	/// </summary>
	/// <param name="source">The source name.</param>
	/// <returns>The id of the mapped emoji.</returns>
	public static ulong GetEmojiBasedOnSourceName(this string source)
	{
#if DEBUG
		return source.ToLowerInvariant() switch
		{
			"applemusic" => 1336837190805885050,
			"yandexmusic" => 1336836824420716545,
			"flowerytts" => 1336836652911300680,
			"deezer" => 1336836375948824576,
			"vkmusic" => 1336836363265511455,
			"soundcloud" => 1336836056225681522,
			"bandcamp" => 1336835936499011615,
			"http" => 1336835679748882433,
			"twitch" => 1336835463293702267,
			"vimeo" => 1336835333358092411,
			"nico" => 1336835252076941342,
			"youtube" => 1336834499903881327,
			"spotify" => 1336834471277760582,
			"local" => 1336838168170987520,
			"bilibili" => 1336840488786985013,
			_ => 1336839088678113482
		};
#else
			return source.ToLowerInvariant() switch
			{
				"spotify" => 1336571943687688252,
				"youtube" => 1336587088132440115,
				_ => 1336624959207903283
			};
#endif
	}
}
