using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("action", "Actions", allowedContexts: [InteractionContextType.Guild, InteractionContextType.PrivateChannel], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall, ApplicationCommandIntegrationTypes.UserInstall]), DeferResponseAsync]
internal class ActionCommands : ApplicationCommandsModule
{
	[SlashCommand("hug", "Hug someone!")]
	public static async Task HugAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## A wild hug appears!";
		var content = $"{ctx.User.Mention} hugs {user.Mention} uwu";
		if (!(await ctx.Client.RestClient.GetWeebShAsync("hug")).TryGetWeebShImage(out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("kiss", "Kiss someone!")]
	public static async Task KissAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## A kiss!";
		var content = $"{ctx.User.Mention} kisses {user.Mention} >~<";
		if (!(await ctx.Client.RestClient.GetWeebShAsync("kiss")).TryGetWeebShImage(out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("lick", "Lick someone!")]
	public static async Task LickAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Slurp~";
		var content = $"{ctx.User.Mention} licks {user.Mention} owo";
		if (!(await ctx.Client.RestClient.GetWeebShAsync("lick")).TryGetWeebShImage(out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("pat", "Pat someone!")]
	public static async Task PatAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Pat pat~";
		var content = $"{ctx.User.Mention} pats {user.Mention} #w#";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("pat", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("poke", "Poke someone!")]
	public static async Task PokeAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Poke poke!";
		var content = $"{ctx.User.Mention} pokes {user.Mention} ÓwÒ";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("poke", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("slap", "Slap someone!")]
	public static async Task SlapAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Slap!";
		var content = $"{ctx.User.Mention} slaps {user.Mention} ÒwÓ";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("slap", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("bite", "Bite someone!")]
	public static async Task BiteAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Bite >:3";
		var content = $"{ctx.User.Mention} bites {user.Mention} >:3";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("bite", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("nom", "Nom someone!")]
	public static async Task NomAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Nom nom~";
		var content = $"{ctx.User.Mention} noms {user.Mention} >:3c";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("nom", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}

	[SlashCommand("stare", "Stare at someone!")]
	public static async Task StateAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
	{
		var title = "## Stare O.o";
		var content = $"{ctx.User.Mention} stares at {user.Mention} O.o";
		if (!ctx.TryGetWeebNetImage(await HatsuneMikuBot.WeebClient.GetRandomAsync("stare", []), out var img))
			await ctx.ActionRespondWithErrorAsync(content, user);
		else if (!await ctx.TryBuildV2ActionMessageAsync(img, title, content, user))
			await ctx.SendOldStyleMessageAsync(img, content, user);
	}
}
