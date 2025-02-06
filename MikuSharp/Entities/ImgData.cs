namespace MikuSharp.Entities;

/// <summary>
///     Represents image data.
/// </summary>
public class ImgData
{
	/// <summary>
	///     Gets the data.
	/// </summary>
	[JsonIgnore]
	public Stream Data { get; set; }

	/// <summary>
	///     Gets the file type.
	/// </summary>
	[JsonIgnore]
	public string Filetype { get; set; }

	/// <summary>
	///     Gets the embed.
	/// </summary>
	[JsonIgnore]
	public DiscordEmbed Embed { get; set; }
}
