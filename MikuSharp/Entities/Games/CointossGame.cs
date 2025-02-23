using MikuSharp.Utilities;

namespace MikuSharp.Entities.Games;

/// <summary>
///     Represents a coin toss game.
/// </summary>
/// <param name="context">The context.</param>
public sealed class CointossGame(InteractionContext context)
{
	/// <summary>
	///     Gets the interaction context.
	/// </summary>
	public InteractionContext Context { get; } = context;

	/// <summary>
	///     Gets the dictionary of coin toss side emojis.
	/// </summary>
	public Dictionary<CointossSide, DiscordEmoji> CoinTossSideEmojis
		=> new()
		{
			{ CointossSide.Heads, DiscordEmoji.FromApplicationEmote(this.Context.Client, 1338091202188279818) },
			{ CointossSide.Tails, DiscordEmoji.FromApplicationEmote(this.Context.Client, 1338091187009228882) }
		};

	/// <summary>
	///     Gets the dictionary of coin toss side images.
	/// </summary>
	public Dictionary<CointossSide, string> CoinTossSideImages { get; } = new()
	{
		{ CointossSide.Heads, "https://miku-cdn.aitsys.dev/assets/miku/cointoss/heads.png" },
		{ CointossSide.Tails, "https://miku-cdn.aitsys.dev/assets/miku/cointoss/tails.png" }
	};

	/// <summary>
	///     Gets or sets the result of the coin toss.
	/// </summary>
	public CointossSide Result { get; internal set; }

	/// <summary>
	///     Tosses the coin and sets the result.
	/// </summary>
	/// <returns>The current instance of <see cref="CointossGame" />.</returns>
	public CointossGame TossCoin()
	{
		Random random = new();
		this.Result = Enum.GetValues<CointossSide>()[random.Next(0, 20) > 10 ? 1 : 0];
		return this;
	}

	/// <summary>
	///     Tries to build and send a components V2 cointoss message.
	/// </summary>
	/// <returns>Whether the message was sent successfully.</returns>
	public async Task<bool> TryBuildV2CointossMessageAsync()
	{
		if (this.Context.GuildId is not 1317206872763404478)
			return false;

		DiscordSectionComponent sectionComponent = new([DiscordExtensionMethods.EmptyComponent, new("And the winner..."), new($"### {this.Result}")]);
		sectionComponent.WithThumbnailComponent(this.CoinTossSideImages[this.Result], this.Result.ToString());
		await this.Context.EditResponseAsync(new DiscordWebhookBuilder().WithV2Components().AddComponents(new DiscordContainerComponent([sectionComponent])));
		return true;
	}

	/// <summary>
	///     Sends an old-style embed cointoss message.
	/// </summary>
	public async Task SendOldStyleCointossMessageAsync()
		=> await this.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{this.CoinTossSideEmojis[this.Result]} It's {this.Result}!"));
}

/// <summary>
///     Represents the sides of a coin.
/// </summary>
public enum CointossSide
{
	/// <summary>
	///     Heads side of the coin.
	/// </summary>
	Heads,

	/// <summary>
	///     Tails side of the coin.
	/// </summary>
	Tails
}
