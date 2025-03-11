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
	public MemoryStream Data { get; set; }

	/// <summary>
	///     Gets the file type.
	/// </summary>
	[JsonIgnore]
	public string FileType { get; set; }
}
