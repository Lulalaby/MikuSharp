namespace MikuSharp.Entities;

/// <summary>
///     Represents a KSoft.si image.
/// </summary>
public sealed class KsoftSiImage : ImgData
{
	/// <summary>
	///     Gets the url.
	/// </summary>
	[JsonProperty("url")]
	public string Url { get; set; }

	/// <summary>
	///     Gets the snowflake.
	/// </summary>
	[JsonProperty("snowflake")]
	public string Snowflake { get; set; }

	/// <summary>
	///     Gets whether the image is NSFW.
	/// </summary>
	[JsonProperty("nsfw")]
	public bool Nsfw { get; set; }

	/// <summary>
	///     Gets the tag.
	/// </summary>
	[JsonProperty("tag")]
	public string Tag { get; set; }
}
