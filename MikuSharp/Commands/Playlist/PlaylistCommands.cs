using MikuSharp.Attributes;

namespace MikuSharp.Commands.Playlist;

/// <summary>
///     The playlist commands
/// </summary>
[SlashCommandGroup("playlist", "Playlist commands", allowedContexts: [InteractionContextType.Guild], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall]), EnsureLavalinkSession]
public partial class PlaylistCommands : ApplicationCommandsModule;
