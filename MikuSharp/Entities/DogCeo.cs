namespace MikuSharp.Entities;

public sealed class DogCeo : ImgData
{
	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("message")]
	public string Message { get; set; }
}
