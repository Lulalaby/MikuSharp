using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("weeb", "Weeb Stuff!", allowedContexts: [InteractionContextType.Guild, InteractionContextType.PrivateChannel], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall, ApplicationCommandIntegrationTypes.UserInstall]), DeferResponseAsync]
internal class WeebCommands : ApplicationCommandsModule
{
	[SlashCommand("diva", "Radnom PJD Loading image")]
	public static async Task DivaPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/diva")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("gumi", "Random Gumi image")]
	public static async Task GumiPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/teto")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("kaito", "Random Kaito image")]
	public static async Task KaitoPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/teto")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("len", "Random Len image")]
	public static async Task KLenPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/len")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("luka", "Random Luka image")]
	public static async Task LukaPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/luka")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("meiko", "Random Meiko image")]
	public static async Task MeikoPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/meiko")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("miku", "Random Miku image")]
	public static async Task HMikuPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/miku")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("rin", "Random Rin image")]
	public static async Task KRinPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/rin")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}

	[SlashCommand("teto", "Random Teto image")]
	public static async Task KTetoPic(InteractionContext ctx)
	{
		if (!(await ctx.Client.RestClient.GetMeekMoeAsync("https://api.meek.moe/teto")).TryGetMeekMoeImage(out var img))
		{
			await ctx.ImageRespondWithErrorAsync();
			return;
		}

		await ctx.SendWeebMessageAsync(img, $"Via {"api.meek.moe".MaskedUrl(new("https://api.meek.moe/"))}");
	}
}
