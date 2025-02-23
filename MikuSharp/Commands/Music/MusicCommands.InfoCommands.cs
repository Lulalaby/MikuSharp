using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Music;

public partial class MusicCommands
{
	/// <summary>
	///     The info commands.
	/// </summary>
	[SlashCommandGroup("info", "Music info commands"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class InfoCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Shows the currently playing song.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("now_playing", "Show whats currently playing")]
		public static async Task ShowNowPlaylingAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Now playing")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Shows the last played song.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("last_played", "Show what played before")]
		public static async Task ShowLastPlayedAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Last played")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}

		/// <summary>
		///     Shows the last playing songs.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("last_playing", "Show what songs were played before")]
		public static async Task ShowLastPlayingAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) => { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Last playing list")); },
				async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")));
		}
	}
}
