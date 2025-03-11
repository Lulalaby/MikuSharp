using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[RequireNsfw, NotDiscordStaff]
public class NsfwCommands : BaseCommandModule
{
	[Command("4k"), Description("lewd")]
	public async Task FourK(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=4k");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("anal"), Description("lewd")]
	public async Task Anal(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=anal");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("ass"), Description("lewd")]
	public async Task Ass(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=ass");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("gonewild"), Description("lewd")]
	public async Task Gonewild(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=gonewild");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("lewdkitsune"), Description("lewd")]
	public async Task LewdKitsune(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=lewdkitsune");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("lewdneko"), Description("lewd")]
	public async Task LewdNeko(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=lewdneko");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("porngif"), Description("lewd")]
	public async Task PornGif(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=pgif");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("pussy"), Description("lewd")]
	public async Task Pussy(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=pussy");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}

	[Command("thighs"), Aliases("thigh"), Description("lewd")]
	public async Task Thighs(CommandContext ctx)
	{
		var img = await ctx.Client.RestClient.GetNsfwNekobotAsync("https://nekobot.xyz/api/image?type=thigh");
		if (img is null)
		{
			await ctx.RespondAsync("Failed to get image");
			await ctx.Message.DeleteAsync("Error while executing command");
			return;
		}

		DiscordMessageBuilder builder = new();
		builder.AddFile($"image.{img.FileType}", img.Data);
		//builder.AddEmbed(img.Embed);
		await ctx.RespondAsync(builder);
	}
}
