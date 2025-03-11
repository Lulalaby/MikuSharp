namespace MikuSharp.Entities;

/// <summary>
///     Represents a meek moe image response.
/// </summary>
public sealed class MeekMoeImage : ImgData
{
	/// <summary>
	///     Gets the url.
	/// </summary>
	[JsonProperty("url")]
	public required string Url { get; set; }

	/// <summary>
	///     Gets the creator.
	/// </summary>
	[JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
	public string? Creator { get; set; }
}
