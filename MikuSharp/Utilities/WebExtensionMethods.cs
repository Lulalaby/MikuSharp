using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

using HeyRed.Mime;

using MikuSharp.Entities;

using Weeb.net;

namespace MikuSharp.Utilities;

/// <summary>
///     Provides extension methods for web-related operations.
/// </summary>
public static class WebExtensionMethods
{
	/// <summary>
	///     Gets a random image from nekos.life.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="url">The url.</param>
	/// <returns>The nekos.life response.</returns>
	public static async Task<NekosLifeImage?> GetNekosLifeAsync(this HttpClient client, string url)
	{
		var dl = JsonConvert.DeserializeObject<NekosLifeImage>(await client.GetStringAsync(url));
		if (dl is null)
			return null;

		MemoryStream str = new(await client.GetByteArrayAsync(dl.Url.ResizeLink()))
		{
			Position = 0
		};
		dl.Data = str;
		dl.FileType = MimeGuesser.GuessExtension(str);
		return dl;
	}

	/// <summary>
	///     Generates an image using the Nekobot API.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <param name="type">The type of image to generate.</param>
	/// <param name="parameters">The parameters.</param>
	public static async Task GenerateNekobotImageAsync(this BaseContext ctx, string type, Dictionary<string, string> parameters)
	{
		var query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
		var response = await ctx.Client.RestClient.GetStringAsync($"https://nekobot.xyz/api/imagegen?type={type}&{query}");
		var result = JsonConvert.DeserializeObject<NekoBotImage?>(response);

		if (result is null)
		{
			await ctx.EditResponseAsync("Something went wrong while fetching the image.");
			return;
		}

		MemoryStream stream = new(await ctx.Client.RestClient.GetByteArrayAsync(result.Message.ResizeLink()))
		{
			Position = 0
		};

		await ctx.SendActionMessageAsync(stream);

		await stream.DisposeAsync();
	}

	/// <summary>
	///     Gets a random image from nekobot.xyz.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="url">The url.</param>
	/// <returns>The nekobot response.</returns>
	public static async Task<NekoBotImage?> GetNsfwNekobotAsync(this HttpClient client, string url)
	{
		var dl = JsonConvert.DeserializeObject<NekoBotImage>(await client.GetStringAsync(url));
		if (dl is null)
			return null;

		MemoryStream str = new(await client.GetByteArrayAsync(dl.Message.ResizeLink()))
		{
			Position = 0
		};
		dl.Data = str;
		dl.FileType = MimeGuesser.GuessExtension(str);
		return dl;
	}

	/// <summary>
	///     Gets an image from meek.moe.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="url">The url.</param>
	/// <returns>The meek.moe response.</returns>
	public static async Task<MeekMoeImage?> GetMeekMoeAsync(this HttpClient client, string url)
	{
		var mm = JsonConvert.DeserializeObject<MeekMoeImage?>(await client.GetStringAsync(url));
		if (mm is null)
			return null;

		var img = new MemoryStream(await client.GetByteArrayAsync(mm.Url.ResizeLink()))
		{
			Position = 0
		};

		mm.Data = img;
		mm.FileType = MimeGuesser.GuessExtension(img);
		return mm;
	}

	/// <summary>
	///     Gets a random image from weeb.sh.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="query">The query.</param>
	/// <param name="tags">The optional tags.</param>
	/// <param name="nsfw">Whether the search should include NSFW results.</param>
	/// <returns>The weeb.sh response.</returns>
	public static async Task<WeebSh?> GetWeebShAsync(this HttpClient client, string query, IEnumerable<string>? tags = null, NsfwSearch nsfw = NsfwSearch.False)
	{
		var dl = await HatsuneMikuBot.WeebClient.GetRandomAsync(query, tags ?? [""], nsfw: nsfw);
		if (dl is null)
			return null;

		MemoryStream img = new(await client.GetByteArrayAsync(dl.Url))
		{
			Position = 0
		};
		var em = new DiscordEmbedBuilder();
		em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
		em.WithFooter("by weeb.sh");
		return new()
		{
			ImgData = img,
			Extension = MimeGuesser.GuessExtension(img),
			Embed = em
		};
	}

	/// <summary>
	///     Tries to get an image from the Weeb.sh API.
	/// </summary>
	/// <param name="data">The data.</param>
	/// <param name="stream">The stream.</param>
	/// <returns>Whether the image was successfully retrieved.</returns>
	public static bool TryGetWeebShImage(this WeebSh? data, [NotNullWhen(true)] out MemoryStream? stream)
	{
		if (data is null)
		{
			stream = null;
			return false;
		}

		stream = data.ImgData;
		return true;
	}

	/// <summary>
	///     Tries to get an image from the Weeb.sh API.
	/// </summary>
	/// <param name="data">The data.</param>
	/// <param name="image">The image.</param>
	/// <returns>Whether the image was successfully retrieved.</returns>
	public static bool TryGetMeekMoeImage(this MeekMoeImage? data, [NotNullWhen(true)] out MeekMoeImage? image)
	{
		if (data is null)
		{
			image = null;
			return false;
		}

		image = data;
		return true;
	}
}
