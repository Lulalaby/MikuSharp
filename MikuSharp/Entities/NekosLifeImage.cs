namespace MikuSharp.Entities;

/// <summary>
///    Represents a nekos.life response.
/// </summary>
public sealed class NekosLifeImage : ImgData
{
	/// <summary>
	///     Gets the URL.
	/// </summary>
	[JsonProperty("url")]
	public string Url { get; set; }
}
