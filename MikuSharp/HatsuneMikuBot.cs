using DisCatSharp.ApplicationCommands.Exceptions;

using DiscordBotsList.Api;

using MikuSharp.Attributes;
using MikuSharp.Commands;
using MikuSharp.Commands.Music;
using MikuSharp.Entities;

using Serilog.Events;

using Weeb.net;

using ActionCommands = MikuSharp.Commands.ActionCommands;
using MikuGuild = MikuSharp.Events.MikuGuild;
using PlaylistCommands = MikuSharp.Commands.Playlist.PlaylistCommands;
using TokenType = DisCatSharp.Enums.TokenType;

namespace MikuSharp;

/// <summary>
///     The Hatsune Miku bot.
/// </summary>
public sealed class HatsuneMikuBot : IDisposable
{
	/// <summary>
	///     Gets the Weeb client.
	/// </summary>
	internal static readonly WeebClient WeebClient = new("Hatsune Miku Bot", "5.0.0");

	/// <summary>
	///     Gets the music sessions.
	/// </summary>
	internal static readonly ConcurrentDictionary<ulong, MusicSession> MusicSessions = [];

	/// <summary>
	///     Gets the music session locks.
	/// </summary>
	internal static readonly ConcurrentDictionary<ulong, AsyncLock> MusicSessionLocks = [];

	/// <summary>
	///     Initializes a new instance of the <see cref="HatsuneMikuBot" /> class.
	/// </summary>
	/// <exception cref="ArgumentNullException">Thrown when the config file is null or missing.</exception>
	public HatsuneMikuBot()
	{
		var fileData = File.ReadAllText("config.json") ?? throw new ArgumentNullException(null, "config.json is null or missing");

		var config = JsonConvert.DeserializeObject<BotConfig>(fileData);
		ArgumentNullException.ThrowIfNull(config);
		Config = config;

		Config.DbConnectString = $"Host={Config.DbConfig.Hostname};Username={Config.DbConfig.User};Password={Config.DbConfig.Password};Database={Config.DbConfig.Database}";
		MikuCancellationTokenSource = new();

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.File("miku_log.txt", fileSizeLimitBytes: null, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 2, shared: true)
			.WriteTo.Console(LogEventLevel.Debug)
			.CreateLogger();
		Log.Logger.Information("Starting up!");

		ShardedClient = new(new()
		{
			Token = Config.DiscordToken,
			TokenType = TokenType.Bot,
			MinimumLogLevel = LogLevel.Debug,
			AutoReconnect = true,
			ApiChannel = ApiChannel.Canary,
			HttpTimeout = TimeSpan.FromMinutes(1),
			Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers,
			MessageCacheSize = 2048,
			LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger),
			ShowReleaseNotesInUpdateCheck = false,
			IncludePrereleaseInUpdateCheck = true,
			DisableUpdateCheck = true,
			EnableSentry = true,
			FeedbackEmail = "aiko@aitsys.dev",
			DeveloperUserId = 856780995629154305,
			AttachUserInfo = true,
			ReconnectIndefinitely = true
		});

		this.InteractivityModules = ShardedClient.UseInteractivityAsync(new()
		{
			Timeout = TimeSpan.FromMinutes(2),
			PaginationBehaviour = PaginationBehaviour.WrapAround,
			PaginationDeletion = PaginationDeletion.DeleteEmojis,
			PollBehaviour = PollBehaviour.DeleteEmojis,
			AckPaginationButtons = true,
			ButtonBehavior = ButtonPaginationBehavior.Disable,
			PaginationButtons = new()
			{
				SkipLeft = new(ButtonStyle.Primary, "pgb-skip-left", "First", false, new("⏮️")),
				Left = new(ButtonStyle.Primary, "pgb-left", "Previous", false, new("◀️")),
				Stop = new(ButtonStyle.Danger, "pgb-stop", "Cancel", false, new("⏹️")),
				Right = new(ButtonStyle.Primary, "pgb-right", "Next", false, new("▶️")),
				SkipRight = new(ButtonStyle.Primary, "pgb-skip-right", "Last", false, new("⏭️"))
			},
			ResponseMessage = "Something went wrong.",
			ResponseBehavior = InteractionResponseBehavior.Ignore
		}).Result;

		this.ApplicationCommandsModules = ShardedClient.UseApplicationCommandsAsync(new()
		{
			EnableDefaultHelp = true,
			DebugStartup = false,
			EnableLocalization = false,
			GenerateTranslationFilesOnly = false
		}).Result;

		this.CommandsNextModules = ShardedClient.UseCommandsNextAsync(new()
		{
			CaseSensitive = false,
			EnableMentionPrefix = true,
			DmHelp = false,
			EnableDefaultHelp = true,
			IgnoreExtraArguments = true,
			StringPrefixes = [],
			UseDefaultCommandHandler = true,
			DefaultHelpChecks = [new NotDiscordStaffAttribute()]
		}).Result;

		this.LavalinkConfig = new()
		{
			SocketEndpoint = new()
			{
				Hostname = Config.LavaConfig.Hostname,
				Port = Config.LavaConfig.Port
			},
			RestEndpoint = new()
			{
				Hostname = Config.LavaConfig.Hostname,
				Port = Config.LavaConfig.Port
			},
			Password = Config.LavaConfig.Password,
			EnableBuiltInQueueSystem = true,
			QueueEntryFactory = () => new MusicQueueEntry()
		};

		this.LavalinkModules = ShardedClient.UseLavalinkAsync().Result;

		DiscordBotListApi = new(ShardedClient.CurrentApplication.Id, Config.DiscordBotListToken);
	}

	/// <summary>
	///     Gets the cancellation token source.
	/// </summary>
	internal static CancellationTokenSource MikuCancellationTokenSource { get; set; }

	/// <summary>
	///     Gets the bot configuration.
	/// </summary>
	internal static BotConfig Config { get; set; }

	/// <summary>
	///     Gets the Lavalink configuration.
	/// </summary>
	internal LavalinkConfiguration LavalinkConfig { get; set; }

	/// <summary>
	///     Gets or sets the game set thread.
	/// </summary>
	internal Task? GameSetThread { get; set; }

	/// <summary>
	///     Gets or sets the bot list thread.
	/// </summary>
	internal Task? BotListThread { get; set; }

	/// <summary>
	///     Gets the Discord Bot List API.
	/// </summary>
	internal static AuthDiscordBotListApi DiscordBotListApi { get; set; }

	/// <summary>
	///     Gets the sharded client.
	/// </summary>
	internal static DiscordShardedClient ShardedClient { get; set; }

	/// <summary>
	///     Gets the interactivity modules.
	/// </summary>
	internal IReadOnlyDictionary<int, InteractivityExtension> InteractivityModules { get; set; }

	/// <summary>
	///     Gets the application commands modules.
	/// </summary>
	internal IReadOnlyDictionary<int, ApplicationCommandsExtension> ApplicationCommandsModules { get; set; }

	/// <summary>
	///     Gets the commands next modules.
	/// </summary>
	internal IReadOnlyDictionary<int, CommandsNextExtension> CommandsNextModules { get; set; }

	/// <summary>
	///     Gets the Lavalink modules.
	/// </summary>
	internal IReadOnlyDictionary<int, LavalinkExtension> LavalinkModules { get; set; }

	/// <inheritdoc />
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		ShardedClient = null!;
	}

	/// <summary>
	///     Registers the events.
	/// </summary>
	internal static async Task RegisterEventsAsync()
	{
		ShardedClient.ClientErrored += (sender, args) =>
		{
			sender.Logger.LogError(args.Exception.Message);
			sender.Logger.LogError(args.Exception.StackTrace);
			return Task.CompletedTask;
		};

		await Task.Delay(1);

		foreach (var discordClientKvp in ShardedClient.ShardClients)
		{
			//discordClientKvp.Value.VoiceStateUpdated += VoiceChat.LeftAlone;

			discordClientKvp.Value.GetApplicationCommands().ApplicationCommandsModuleStartupFinished += (sender, args) =>
			{
				sender.Client.Logger.LogInformation("Shard {shard} finished application command startup.", args.ShardId);
				args.Handled = true;
				return Task.CompletedTask;
			};

			discordClientKvp.Value.GetApplicationCommands().ApplicationCommandsModuleReady += (sender, args) =>
			{
				sender.Client.Logger.LogInformation("Application commands module is ready");
				args.Handled = true;
				return Task.CompletedTask;
			};

			discordClientKvp.Value.GetApplicationCommands().SlashCommandErrored += async (sender, args) =>
			{
				if (args.Exception is SlashExecutionChecksFailedException ex)
					if (ex.FailedChecks.Any(x => x is ApplicationCommandRequireTeamMemberAttribute))
					{
						await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("This command is limit to developers"));
						return;
					}

				await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("An error occurred while executing this command."));
			};

			discordClientKvp.Value.GetApplicationCommands().ContextMenuErrored += async (sender, args) =>
			{
				if (args.Exception is SlashExecutionChecksFailedException ex)
					if (ex.FailedChecks.Any(x => x is ApplicationCommandRequireTeamMemberAttribute))
					{
						await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("This command is limit to developers"));
						return;
					}

				await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent("An error occurred while executing this command."));
			};

			discordClientKvp.Value.GuildMemberUpdated += async (sender, args) =>
			{
				if (args.Guild.Id == 483279257431441410)
					await MikuGuild.OnUpdateAsync(sender, args);
				else
					await Task.FromResult(true);
			};

			discordClientKvp.Value.Logger.LogInformation("Registered events for shard {shard}", discordClientKvp.Value.ShardId);
		}
	}

	/// <summary>
	///     Updates the bot list statistics.
	/// </summary>
	internal static async Task UpdateBotListStatisticsAsync()
	{
		await Task.Delay(15000);

		while (true)
		{
			var me = await DiscordBotListApi.GetMeAsync();
			var count = Array.Empty<int>();
			var clients = ShardedClient.ShardClients.Values;
			count = clients.Aggregate(count, (current, client) => [.. current, client.Guilds.Count]);
			await me.UpdateStatsAsync(0, ShardedClient.ShardClients.Count, count);
			await Task.Delay(TimeSpan.FromMinutes(15));
		}
	}

	/// <summary>
	///     Rotates the activity every <c>20</c> minutes.
	/// </summary>
	internal static async Task RotateActivityAsync()
	{
		while (true)
		{
			DiscordActivity firstActivity = new()
			{
				Name = "New music system coming up soon!",
				ActivityType = ActivityType.Playing
			};
			await ShardedClient.UpdateStatusAsync(firstActivity, UserStatus.Online);
			await Task.Delay(TimeSpan.FromMinutes(20));
			DiscordActivity secondActivity = new()
			{
				Name = "Mention me with help for other commands!",
				ActivityType = ActivityType.Playing
			};
			await ShardedClient.UpdateStatusAsync(secondActivity, UserStatus.Online);
			await Task.Delay(TimeSpan.FromMinutes(20));
			DiscordActivity thirdActivity = new()
			{
				Name = "Full NND support!",
				ActivityType = ActivityType.Playing
			};
			await ShardedClient.UpdateStatusAsync(thirdActivity, UserStatus.Online);
			await Task.Delay(TimeSpan.FromMinutes(20));
		}
	}

	/// <summary>
	///     Registers the commands.
	/// </summary>
	internal void RegisterCommands()
	{
		// Nsfw stuff needs to be hidden, that's why we use commands next
		this.CommandsNextModules.RegisterCommands<NsfwCommands>();

		this.ApplicationCommandsModules.RegisterGlobalCommands<ActionCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<DeveloperOnlyCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<FunCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<AboutCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<ModerationCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<MusicCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<PlaylistCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<UtilityCommands>();
		this.ApplicationCommandsModules.RegisterGlobalCommands<WeebCommands>();

		// Smolcar command, miku discord guild command
		this.ApplicationCommandsModules.RegisterGuildCommands<MikuGuildCommands>(483279257431441410);
	}

	/// <summary>
	///     Runs the bot.
	/// </summary>
	internal async Task RunAsync()
	{
		await WeebClient.Authenticate(Config.WeebShToken, Weeb.net.TokenType.Wolke);
		await ShardedClient.StartAsync();
		await Task.Delay(5000);

		var success = false;
		while (!success)
			try
			{
				foreach (var lavalinkShard in this.LavalinkModules)
					await lavalinkShard.Value.ConnectAsync(this.LavalinkConfig);
				success = true;
			}
			catch
			{
				success = false;
			}

		this.GameSetThread = Task.Run(RotateActivityAsync);
		//BotListThread = Task.Run(UpdateBotList);
		while (!MikuCancellationTokenSource.IsCancellationRequested)
			await Task.Delay(25);
		_ = this.LavalinkModules.Select(lavalinkShard => lavalinkShard.Value.ConnectedSessions.Select(async connectedSession => await connectedSession.Value.DestroyAsync()));
		await ShardedClient.StopAsync();
	}

	/// <inheritdoc />
	~HatsuneMikuBot()
	{
		this.Dispose();
	}
}
