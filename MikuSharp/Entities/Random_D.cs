namespace MikuSharp.Entities;

public sealed class RandomD : ImgData
{
	[JsonProperty("url")]
	public string Url { get; set; }

	[JsonProperty("message")]
	public string Message { get; set; }
}
