namespace MikuSharp.Entities;

/// <summary>
///     Represents a meek moe image response.
/// </summary>
public sealed class MeekMoeImage : ImgData
{
	[JsonProperty("url")]
	public string Url { get; set; }

	[JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
	public string? Creator { get; set; }
}
