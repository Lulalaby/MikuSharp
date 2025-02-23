using MikuSharp.Attributes;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Music;

public partial class MusicCommands
{
	/// <summary>
	///     The options commands.
	/// </summary>
	[SlashCommandGroup("options", "Music options commands"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public class OptionsCommands : ApplicationCommandsModule
	{
		/// <summary>
		///     Changes the repeat mode.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="mode">The new repeat mode.</param>
		[SlashCommand("repeat", "Repeat the current song or the entire queue")]
		public static async Task RepeatAsync(
			InteractionContext ctx,
			[Option("mode", "New repeat mode"), ChoiceProvider(typeof(FixedOptionProviders.RepeatModeProvider))]
			RepeatMode mode
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.UpdateRepeatMode(mode);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed());
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Set repeat mode to: **{mode}**"));
			});
		}

		/// <summary>
		///     Shuffles the queue.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		[SlashCommand("shuffle", "Shuffle the queue")]
		public static async Task ShuffleAsync(InteractionContext ctx)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				musicSession.LavalinkGuildPlayer.ShuffleQueue();
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Shuffled the queue!"));
			});
		}

		/// <summary>
		///     Changes the volume of the music player.
		/// </summary>
		/// <param name="ctx">The interaction context.</param>
		/// <param name="volume">The volume to set.</param>
		[SlashCommand("volume", "Change the music volume")]
		public static async Task ModifyVolumeAsync(
			InteractionContext ctx,
			[Option("volume", "Level of volume to set (Percentage)"), MinimumValue(0), MaximumValue(150)]
			int volume = 100
		)
		{
			await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				await musicSession.LavalinkGuildPlayer.SetVolumeAsync(volume);
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed());
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Set the volume to **{volume}%**!"));
			});
		}
	}
}
