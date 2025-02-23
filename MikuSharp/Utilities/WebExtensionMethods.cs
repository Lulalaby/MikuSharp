using System.Net.Http;

using HeyRed.Mime;

using MikuSharp.Entities;

using Weeb.net;

namespace MikuSharp.Utilities;

/// <summary>
///    Provides extension methods for web-related operations.
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
		dl.Filetype = MimeGuesser.GuessExtension(str);
		var em = new DiscordEmbedBuilder();
		em.WithImageUrl($"attachment://image.{dl.Filetype}");
		em.WithFooter("by nekos.life");
		dl.Embed = em.Build();
		return dl;
	}

	/// <summary>
	///     Gets a random image from ksoft.si.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="tag">The tag.</param>
	/// <param name="nsfw">Whether the search should include NSFW results.</param>
	/// <returns>The ksoft.si response.</returns>
	public static async Task<KsoftSiImage?> GetKsoftSiImgageAsync(this HttpClient client, string tag = "hentai_gif", bool nsfw = true)
	{
		client.DefaultRequestHeaders.Authorization = new("Bearer", HatsuneMikuBot.Config.KsoftSiToken);
		var dl = JsonConvert.DeserializeObject<KsoftSiImage>(await client.GetStringAsync($"https://api.ksoft.si/images/random-image?tag={tag}&nsfw={nsfw.ToString().ToLowerInvariant()}"));
		if (dl is null)
			return null;

		MemoryStream img = new(await client.GetByteArrayAsync(dl.Url.ResizeLink()));
		dl.Data = img;
		dl.Filetype = MimeGuesser.GuessExtension(img);
		var em = new DiscordEmbedBuilder();
		em.WithImageUrl($"attachment://image.{dl.Filetype}");
		em.WithFooter("by KSoft.si");
		dl.Embed = em.Build();
		return dl;
	}

	/// <summary>
	/// Generates an image using the Nekobot API.
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

		if (!await ctx.TryBuildV2ActionMessageAsync(stream))
			await ctx.SendOldStyleMessageAsync(stream);

		await stream.DisposeAsync();
	}

	/// <summary>
	///     Gets a random image from nekobot.xyz.
	/// </summary>
	/// <param name="client">The http client.</param>
	/// <param name="url">The url.</param>
	/// <returns>The nekobot response.</returns>
	public static async Task<NekoBotImage?> GetNekobotAsync(this HttpClient client, string url)
	{
		var dl = JsonConvert.DeserializeObject<NekoBotImage>(await client.GetStringAsync(url));
		if (dl is null)
			return null;

		MemoryStream str = new(await client.GetByteArrayAsync(dl.Message.ResizeLink()))
		{
			Position = 0
		};
		dl.Data = str;
		dl.Filetype = MimeGuesser.GuessExtension(str);
		var em = new DiscordEmbedBuilder();
		em.WithImageUrl($"attachment://image.{dl.Filetype}");
		em.WithFooter("by nekobot.xyz");
		dl.Embed = em.Build();
		return dl;
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
}
