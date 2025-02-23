using System.Net;

namespace MikuSharp.Entities;

/// <summary>
///     Represents a Nekobot response.
/// </summary>
public sealed class NekoBotImage : ImgData
{
	/// <summary>
	///     Gets the message.
	/// </summary>
	[JsonProperty("message", NullValueHandling = NullValueHandling.Include)]
	public string Message { get; internal set; }

	/// <summary>
	///     Gets the status.
	/// </summary>
	[JsonProperty("status")]
	public HttpStatusCode Status { get; internal set; }

	/// <summary>
	///     Gets whether the request was successful.
	/// </summary>
	[JsonProperty("success")]
	public bool Success { get; internal set; }

	/// <summary>
	///     Gets the version.
	/// </summary>
	[JsonProperty("version")]
	public string Version { get; internal set; }
}
