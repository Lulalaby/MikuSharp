using DisCatSharp.Exceptions;

using MikuSharp.Attributes;
using MikuSharp.Entities;
using MikuSharp.Utilities;

namespace MikuSharp.Commands.Music;

/// <summary>
///     The music commands
/// </summary>
[SlashCommandGroup("music", "Music commands", allowedContexts: [InteractionContextType.Guild], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall]), EnsureLavalinkSession]
public partial class MusicCommands : ApplicationCommandsModule
{
	/// <summary>
	///     Joins a voice channel the user is in.
	/// </summary>
	/// <param name="ctx">The interaction context.</param>
	[SlashCommand("join", "Joins the voice channel you're in"), RequireUserVoicechatConnection, AutomaticallyDisconnectExistingSession, DeferResponseAsync(true)]
	public async Task JoinAsync(InteractionContext ctx)
	{
		ArgumentNullException.ThrowIfNull(ctx.Member?.VoiceState?.Channel);
		ArgumentNullException.ThrowIfNull(ctx.Guild);
		await ctx.ExecuteWithMusicSessionAsync(async (_, _) => await ctx.EditResponseAsync("I'm already connected"),
			async guildId =>
			{
				var session = ctx.Client.GetLavalink().DefaultSession();
				ArgumentNullException.ThrowIfNull(session);
				await session.ConnectAsync(ctx.Member.VoiceState.Channel);
				var musicSession = await new MusicSession(ctx.Member.VoiceState.Channel, ctx.Guild, session).InjectPlayerAsync();
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Heya {ctx.Member.Mention}!"));
				await musicSession.CurrentChannel.SendMessageAsync("Hatsune Miku at your service!");
				await musicSession.UpdateStatusMessageAsync(musicSession.BuildMusicStatusEmbed("Nothing playing yet"));
				MikuBot.MusicSessions[guildId] = musicSession;
			});
	}

	/// <summary>
	///     Leaves a voice channel.
	/// </summary>
	/// <param name="ctx">The interaction context.</param>
	[SlashCommand("leave", "Leaves the voice channel"), RequireUserAndBotVoicechatConnection, DeferResponseAsync(true)]
	public async Task LeaveAsync(InteractionContext ctx)
	{
		await ctx.ExecuteWithMusicSessionAsync(async (_, musicSession) =>
			{
				if (musicSession.LavalinkGuildPlayer is not null)
					await musicSession.LavalinkGuildPlayer.DisconnectAsync();
				await musicSession.CurrentChannel.SendMessageAsync("Bye bye humans 💙");
				if (musicSession.StatusMessage is not null)
					await musicSession.StatusMessage.DeleteAsync("Miku disconnected");
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Cya! 💙"));
			},
			async _ => await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not connected O.o")),
			guildId => Task.FromResult(MikuBot.MusicSessionLocks.TryRemove(guildId, out _)));
	}

	[SlashCommand("test", "Test UI Kit"), ApplicationCommandRequireTeamMember, DeferResponseAsync]
	public async Task TestAsync(InteractionContext ctx, [Option("identifier", "The identifier (lavalink identifier, url, etc..)")] string identifier)
	{
		LavalinkTrack track;
		var session = ctx.Client.GetLavalink().DefaultSession()!;
		var loadResult = await session.LoadTracksAsync(identifier);
		switch (loadResult.LoadType)
		{
			case LavalinkLoadResultType.Track:
				track = loadResult.GetResultAs<LavalinkTrack>();
				break;
			case LavalinkLoadResultType.Search:
				var tracks = loadResult.GetResultAs<List<LavalinkTrack>>();
				track = tracks.First();
				break;
			case LavalinkLoadResultType.Playlist:
			case LavalinkLoadResultType.Empty:
			case LavalinkLoadResultType.Error:
			default:
				throw new ArgumentOutOfRangeException(null, "Unsupported result type");
		}

		var artistArtworkUrl = track.PluginInfo.AdditionalProperties.TryGetValue("artistArtworkUrl", out var url1) ? url1.ToString()! : null;
		var albumName = track.PluginInfo.AdditionalProperties.TryGetValue("albumName", out var url2) ? url2.ToString() : null;
		var albumUrl = track.PluginInfo.AdditionalProperties.TryGetValue("albumUrl", out var url3) ? url3.ToString() : null;
		var artistUrl = track.PluginInfo.AdditionalProperties.TryGetValue("artistUrl", out var url4) ? url4.ToString() : null;

		DiscordSeparatorComponent separator = new(false, SeparatorSpacingSize.Small);
		DiscordTextDisplayComponent trackInfo = new($"### {track.Info.Title}\n**Artist:** {track.Info.Author.InlineCode()}\n**Length:** {track.Info.Length.FormatTimeSpan().InlineCode()}{(!string.IsNullOrEmpty(albumName) ? $"\n**Album:** {albumName.InlineCode()}" : "")}");
		DiscordSectionComponent trackInfoSection = new([trackInfo]);
		trackInfoSection.WithThumbnailComponent(artistArtworkUrl!, "Artist Artwork");
		DiscordTextDisplayComponent additionalTrackInfo = new($"-# **ISrc:** {track.Info.Isrc?.InlineCode() ?? "none".InlineCode()} **Identifier:** {track.Info.Identifier.InlineCode()}");
		DiscordMediaGalleryItem trackCover = new(track.Info.ArtworkUrl!.AbsoluteUri, "Song Artwork");
		DiscordMediaGalleryComponent trackMediaGallery = new([trackCover]);
		DiscordLinkButtonComponent trackLink = new(track.Info.Uri.AbsoluteUri, "View Track", emoji: new(GetEmojiBasedOnIdentifier(identifier)));
		DiscordActionRowComponent links1 = new([trackLink]);
		List<DiscordComponent> infoComponents = [string.IsNullOrEmpty(artistArtworkUrl) ? trackInfo : trackInfoSection, separator, trackMediaGallery, separator, links1];
		if (!string.IsNullOrEmpty(albumUrl))
		{
			DiscordLinkButtonComponent albumLink = new(albumUrl, "View Album", emoji: new(GetEmojiBasedOnIdentifier(identifier)));
			DiscordActionRowComponent links2 = new([albumLink]);
			infoComponents.Add(links2);
		}

		if (!string.IsNullOrEmpty(artistUrl))
		{
			DiscordLinkButtonComponent artistLink = new(artistUrl, "View Artist", emoji: new(GetEmojiBasedOnIdentifier(identifier)));
			DiscordActionRowComponent links3 = new([artistLink]);
			infoComponents.Add(links3);
		}

		infoComponents.AddRange([separator, additionalTrackInfo]);

		DiscordContainerComponent infoContainer = new([..infoComponents]);
		try
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithV2Components().AddComponents(infoContainer));
		}
		catch (BadRequestException ex)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(ex.Errors?.BlockCode("json") ?? ex.Message));
		}

		return;

		ulong GetEmojiBasedOnIdentifier(string ident)
			=> ident.ToLowerInvariant().Contains("spotify")
				? 1336571943687688252
				: (ulong)1336587088132440115;
	}
}
