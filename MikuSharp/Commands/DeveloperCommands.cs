using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using MikuSharp.Attributes;

namespace MikuSharp.Commands;

/// <summary>
///     The developer commands.
/// </summary>
[ApplicationCommandRequireTeamMember]
public class DeveloperOnlyCommands : ApplicationCommandsModule
{
	/// <summary>
	///     The units.
	/// </summary>
	private static readonly string[] s_units = ["", "ki", "Mi", "Gi"];

	/// <summary>
	///     Evals a csharp script.
	/// </summary>
	/// <param name="ctx">The context menu context.</param>
	[ContextMenu(ApplicationCommandType.Message, "Eval - Miku Dev"), DeferResponseAsync(true)]
	public static async Task EvalAsync(ContextMenuContext ctx)
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Eval request"));
		var msg = ctx.TargetMessage;
		var code = msg.Content;
		var cs1 = code.IndexOf("```", StringComparison.Ordinal) + 3;
		cs1 = code.IndexOf('\n', cs1) + 1;
		var cs2 = code.LastIndexOf("```", StringComparison.Ordinal);

		if (cs1 is -1 || cs2 is -1)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You need to wrap the code into a code block."));
			return;
		}

		var cs = code[cs1..cs2];

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithColor(new("#FF007F"))
			.WithDescription("Evaluating...\n\nMeanwhile: https://eval-deez-nuts.xyz/")
			.Build())).ConfigureAwait(false);

		try
		{
			var globals = new MikuDeveloperEvalVariables(ctx.TargetMessage, ctx.Client, ctx, HatsuneMikuBot.ShardedClient);

			var sopts = ScriptOptions.Default;
			sopts = sopts.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks", "DisCatSharp", "DisCatSharp.Entities", "DisCatSharp.CommandsNext", "DisCatSharp.CommandsNext.Attributes",
				"DisCatSharp.Interactivity", "DisCatSharp.Interactivity.Extensions", "DisCatSharp.Enums", "Microsoft.Extensions.Logging", "MikuSharp.Entities");
			sopts = sopts.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

			var script = CSharpScript.Create(cs, sopts, typeof(MikuDeveloperEvalVariables));
			script.Compile(HatsuneMikuBot.MikuCancellationTokenSource.Token);
			var result = await script.RunAsync(globals, HatsuneMikuBot.MikuCancellationTokenSource.Token).ConfigureAwait(false);

			if (result is { ReturnValue: not null } && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
				{
					Title = "Evaluation Result",
					Description = result.ReturnValue.ToString(),
					Color = new DiscordColor("#007FFF")
				}.Build())).ConfigureAwait(false);
			else
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
				{
					Title = "Evaluation Successful",
					Description = "No result was returned.",
					Color = new DiscordColor("#007FFF")
				}.Build())).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
			{
				Title = "Evaluation Failure",
				Description = string.Concat("**", ex.GetType().ToString(), "**: ", ex.Message),
				Color = new DiscordColor("#FF0000")
			}.Build())).ConfigureAwait(false);
		}
	}

	/// <summary>
	///     Evals a csharp script. Version 2 with the new components :eyes:.
	/// </summary>
	/// <param name="ctx">The context menu context.</param>
	[ContextMenu(ApplicationCommandType.Message, "Eval V2 - Miku Dev"), DeferResponseAsync]
	public static async Task EvalV2Async(ContextMenuContext ctx)
	{
		var msg = ctx.TargetMessage;
		var code = msg.Content;
		var cs1 = code.IndexOf("```", StringComparison.Ordinal) + 3;
		cs1 = code.IndexOf('\n', cs1) + 1;
		var cs2 = code.LastIndexOf("```", StringComparison.Ordinal);

		if (cs1 is -1 || cs2 is -1)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You need to wrap the code into a code block."));
			return;
		}

		var cs = code[cs1..cs2];

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithColor(new("#FF007F"))
			.WithDescription("Evaluating...\n\nMeanwhile: https://eval-deez-nuts.xyz/")
			.Build())).ConfigureAwait(false);

		try
		{
			var globals = new MikuDeveloperEvalVariables(ctx.TargetMessage, ctx.Client, ctx, HatsuneMikuBot.ShardedClient);

			var sopts = ScriptOptions.Default;
			sopts = sopts.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks", "DisCatSharp", "DisCatSharp.Entities", "DisCatSharp.CommandsNext", "DisCatSharp.CommandsNext.Attributes",
				"DisCatSharp.Interactivity", "DisCatSharp.Interactivity.Extensions", "DisCatSharp.Enums", "Microsoft.Extensions.Logging", "MikuSharp.Entities");
			sopts = sopts.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

			var script = CSharpScript.Create(cs, sopts, typeof(MikuDeveloperEvalVariables));
			script.Compile(HatsuneMikuBot.MikuCancellationTokenSource.Token);
			var result = await script.RunAsync(globals, HatsuneMikuBot.MikuCancellationTokenSource.Token).ConfigureAwait(false);

			if (result is { ReturnValue: not null } && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
				{
					Title = "Evaluation Result",
					Description = result.ReturnValue.ToString(),
					Color = new DiscordColor("#007FFF")
				}.Build())).ConfigureAwait(false);
			else
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
				{
					Title = "Evaluation Successful",
					Description = "No result was returned.",
					Color = new DiscordColor("#007FFF")
				}.Build())).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
			{
				Title = "Evaluation Failure",
				Description = string.Concat("**", ex.GetType().ToString(), "**: ", ex.Message),
				Color = new DiscordColor("#FF0000")
			}.Build())).ConfigureAwait(false);
		}
	}

	/// <summary>
	///     Deletes a message sent by the bot.
	/// </summary>
	/// <param name="ctx">The context menu context.</param>
	[ContextMenu(ApplicationCommandType.Message, "Remove message - Miku Dev"), DeferResponseAsync(true)]
	public static async Task DeleteMessageAsync(ContextMenuContext ctx)
	{
		if (ctx.TargetMessage.Author.Id != ctx.Client.CurrentUser.Id)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You can only delete messages sent by me."));
			return;
		}

		await ctx.TargetMessage.DeleteAsync();
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Done"));
	}

	/// <summary>
	///     Converts a size to a human-readable string.
	/// </summary>
	/// <param name="size">The size to convert.</param>
	/// <returns>A human-readable string representing the size.</returns>
	private static string SizeToString(long size)
	{
		double convertedSize = size;
		var u = 0;

		while (convertedSize >= 900 && u < s_units.Length - 2)
		{
			u++;
			convertedSize /= 1024;
		}

		return $"{convertedSize:#,##0.00} {s_units[u]}B";
	}

	/// <summary>
	///     The developer commands.
	/// </summary>
	[SlashCommandGroup("dev", "Developer commands")]
	public class DeveloperCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     A test command.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("test", "Testing")]
		public static async Task TestAsync(InteractionContext ctx)
		{
			var builder = new DiscordInteractionResponseBuilder().AsEphemeral().AsSilentMessage().SuppressEmbeds().AsVoiceMessage().WithV2Components();
			var response = await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, builder);
			await ctx.EditResponseAsync($"Send the following flags: {response.SendFlags}\nReceived the following flags from the callback response: {response.Message?.Flags}");
			/*await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
			List<DiscordMediaGalleryItem> items = [new("https://cdn.discordapp.com/attachments/1211030818533937362/1338113601453686835/lulalaby_Catgirls_5e44ded1-2d0d-4be7-8e1e-b08400429ec3.png?ex=67a9e6e7&is=67a89567&hm=c2de0d5bedf5f981dd66e707ed9805c4c72d0ae66161ff064a7b698e686d729c&")];
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithV2Components().AddComponents(new DiscordContainerComponent([new DiscordSectionComponent([new("**Catgirlsdsaophifejkgü#äl,lsd gjf bgkfnd lög kjdf gdks flkds fujenaolsf ewj bfiew löf eroiwfb eikmfpsdnifb jkemds wflkoen uje fmj ewofn udesj fckmds mfgoe4wbrhjrf em,  folewbf jew f --s 750 --v 6.1 --p x5nrtis** - <@856780995629154305> (turbo, stealth)".SingleQuote())]).WithThumbnailComponent("https://example.com/image.png"), new DiscordMediaGalleryComponent([..items])])));*/
		}

		/// <summary>
		///     Guild shard test command.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("guild_shard_test", "Testing")]
		public static async Task GuildTestAsync(InteractionContext ctx)
		{
			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"Meep meep. Shard {ctx.Client.ShardId}"));
			foreach (var shard in HatsuneMikuBot.ShardedClient.ShardClients.Values)
				await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral().WithContent($"Shard {shard.ShardId} has {shard.Guilds.Count} guilds."));
		}

		/// <summary>
		///     Gets the Lavalink statistics.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("lstats", "Displays Lavalink statistics"), DeferResponseAsync(true)]
		public static async Task GetLavalinkStatsAsync(InteractionContext ctx)
		{
			var stats = ctx.Client.GetLavalink().ConnectedSessions.First().Value.Statistics;
			var sb = new StringBuilder();
			sb.Append("Lavalink resources usage statistics: ```")
				.Append("Uptime:                    ").Append(stats.Uptime).AppendLine()
				.Append("Players:                   ").Append($"{stats.PlayingPlayers} active / {stats.Players} total").AppendLine()
				.Append("CPU Cores:                 ").Append(stats.Cpu.Cores).AppendLine()
				.Append("CPU Usage:                 ").Append($"{stats.Cpu.LavalinkLoad:#,##0.0%} lavalink / {stats.Cpu.SystemLoad:#,##0.0%} system").AppendLine()
				.Append("RAM Usage:                 ")
				.Append($"{SizeToString(stats.Memory.Allocated)} allocated / {SizeToString(stats.Memory.Used)} used / {SizeToString(stats.Memory.Free)} free / {SizeToString(stats.Memory.Reservable)} reservable").AppendLine()
				.Append("Audio frames (per minute): ").Append($"{stats.Frames?.Sent:#,##0} sent / {stats.Frames?.Nulled:#,##0} nulled / {stats.Frames?.Deficit:#,##0} deficit").AppendLine()
				.Append("```");
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(sb.ToString()));
		}

		/// <summary>
		///     Gets the debug log.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("dbg", "Get the logs of today")]
		public static async Task GetDebugLogAsync(InteractionContext ctx)
		{
			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("Log request"));
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Trying to get log"));
			var now = DateTime.Now;
			var targetFile = $"miku_log{now.ToString("yyyy/MM/dd").Replace("/", "")}.txt";

			if (!File.Exists(targetFile))
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to get log"));
				return;
			}

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Found log {targetFile.Bold()}"));

			try
			{
				if (!File.Exists($"temp-{targetFile}"))
					File.Copy(targetFile, $"temp-{targetFile}");
				else
				{
					File.Delete($"temp-{targetFile}");
					File.Copy(targetFile, $"temp-{targetFile}");
				}

				FileStream log = new($"temp-{targetFile}", FileMode.Open, FileAccess.Read);
				await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddFile(targetFile, log, true).WithContent($"Log {targetFile.Bold()}").AsEphemeral());
				log.Close();
				await log.DisposeAsync();
				File.Delete($"temp-{targetFile}");
			}
			catch (Exception ex)
			{
				await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(ex.Message).AsEphemeral());
				if (ex.StackTrace is not null)
					await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(ex.StackTrace).AsEphemeral());
			}

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Done"));
		}

		/// <summary>
		///     Monetization tests.
		/// </summary>
		[SlashCommandGroup("monetization", "Monetization tests")]
		public class Monetization : ApplicationCommandsModule
		{
			private const ulong CONSUMABLE_SKU_ID = 1337743977473900555;
			private const ulong DURABLE_SKU_ID = 1337744226666151949;

			/// <summary>
			///     Consumes a consumable.
			/// </summary>
			/// <param name="ctx">The interaction context.</param>
			[SlashCommand("consume_consumable", "Consume a consumable"), ApplicationCommandRequireSkuEntitlement(CONSUMABLE_SKU_ID)]
			public static async Task ConsumeConsumableAsync(InteractionContext ctx)
			{
				await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("Testing.."));

				if (ctx.Entitlements.Any(x => x.Id == CONSUMABLE_SKU_ID))
				{
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Consumable works"));
					if (await ctx.Entitlements.First(x => x.Id == CONSUMABLE_SKU_ID).ConsumeAsync())
						await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Consumable consumed"));
					else
						await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Consumable failed to consume"));
				}
				else
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Huh?!"));
			}

			/// <summary>
			///     Uses a durable.
			/// </summary>
			/// <param name="ctx">The interaction context.</param>
			[SlashCommand("use_durable", "Use a durable"), ApplicationCommandRequireSkuEntitlement(DURABLE_SKU_ID)]
			public static async Task UseDurableAsync(InteractionContext ctx)
			{
				await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("Testing.."));

				if (ctx.Entitlements.Any(x => x.Id == DURABLE_SKU_ID))
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Durable works"));
				else
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Huh?!"));
			}
		}
	}
}

/// <remarks>
///     Initializes a new instance of the <see cref="MikuDeveloperEvalVariables" /> class.
/// </remarks>
/// <param name="msg">The message.</param>
/// <param name="client">The client.</param>
/// <param name="ctx">The context menu context.</param>
public sealed class MikuDeveloperEvalVariables(DiscordMessage msg, DiscordClient client, ContextMenuContext ctx, DiscordShardedClient shard)
{
	/// <summary>
	///     Gets or sets the message.
	/// </summary>
	public DiscordMessage Message { get; set; } = msg;

	/// <summary>
	///     Gets or sets the channel.
	/// </summary>
	public DiscordChannel Channel { get; set; } = ctx.Channel;

	/// <summary>
	///     Gets or sets the guild.
	/// </summary>
	public DiscordGuild? Guild { get; set; } = ctx.Guild;

	/// <summary>
	///     Gets or sets the user.
	/// </summary>
	public DiscordUser User { get; set; } = ctx.User;

	/// <summary>
	///     Gets or sets the member.
	/// </summary>
	public DiscordMember? Member { get; set; } = ctx.Member;

	/// <summary>
	///     Gets or sets the context menu context.
	/// </summary>
	public ContextMenuContext Context { get; set; } = ctx;

	/// <summary>
	///     Gets or sets the shard client.
	/// </summary>
	public DiscordShardedClient ShardClient { get; set; } = shard;

	/// <summary>
	///     Gets or sets the client.
	/// </summary>
	public DiscordClient Client { get; set; } = client;
}
