namespace MikuSharp.Entities.Games;

/// <summary>
///     Represents a rock-paper-scissors game.
/// </summary>
public static class RockPaperScissorsGame
{
	/// <summary>
	///     The rock–paper–scissors assets.
	/// </summary>
	public static IEnumerable<RockPaperScissorsAsset> Assets
		=> [new(RockPaperScissorsChoiceType.Rock, "https://miku-cdn.aitsys.dev/assets/miku/rps/noun-rock-88661.png", "Rock by Studio Fibonacci from Noun Project (CC BY 3.0)"), new(RockPaperScissorsChoiceType.Paper, "https://miku-cdn.aitsys.dev/assets/miku/rps/noun-paper-88662.png", "Paper by Studio Fibonacci from Noun Project (CC BY 3.0)"), new(RockPaperScissorsChoiceType.Scissors, "https://miku-cdn.aitsys.dev/assets/miku/rps/noun-scissors-88666.png", "Scissors by Studio Fibonacci from Noun Project (CC BY 3.0)")];

	/// <summary>
	///     Plays a round of rock–paper–scissors against the user.
	/// </summary>
	/// <param name="userChoice">The user's choice.</param>
	/// <returns>A <see cref="RockPaperScissorsResponse" /> representing the outcome.</returns>
	public static RockPaperScissorsResponse ResolveRps(this RockPaperScissorsChoiceType userChoice)
	{
		var random = new Random();
		var values = Enum.GetValues<RockPaperScissorsChoiceType>();
		var computerChoice = (RockPaperScissorsChoiceType)values.GetValue(random.Next(values.Length))!;

		return new(userChoice, computerChoice, userChoice == computerChoice
			? RockPaperScissorsWinType.Tie
			: (userChoice is RockPaperScissorsChoiceType.Rock && computerChoice is RockPaperScissorsChoiceType.Scissors) ||
			  (userChoice is RockPaperScissorsChoiceType.Paper && computerChoice is RockPaperScissorsChoiceType.Rock) ||
			  (userChoice is RockPaperScissorsChoiceType.Scissors && computerChoice is RockPaperScissorsChoiceType.Paper)
				? RockPaperScissorsWinType.User
				: RockPaperScissorsWinType.Computer);
	}
}

/// <summary>
///     Represents a rock-paper-scissors asset.
/// </summary>
/// <param name="choiceType">The type.</param>
/// <param name="imgUrl">The image URL.</param>
/// <param name="ccl">The creative commons license.</param>
public sealed class RockPaperScissorsAsset(RockPaperScissorsChoiceType choiceType, string imgUrl, string ccl)
{
	/// <summary>
	///     The type.
	/// </summary>
	public RockPaperScissorsChoiceType ChoiceType { get; } = choiceType;

	/// <summary>
	///     The image URL.
	/// </summary>
	public string ImageUrl { get; } = imgUrl;

	/// <summary>
	///     The creative commons license.
	/// </summary>
	public string CreativeCommonsLicense { get; } = ccl;

	/// <summary>
	///     Maps a <see cref="RockPaperScissorsChoiceType" /> to a <see cref="RockPaperScissorsAsset" />.
	/// </summary>
	/// <param name="type">The choice type.</param>
	/// <returns>The mapped <see cref="RockPaperScissorsAsset" />.</returns>
	public static implicit operator RockPaperScissorsAsset(RockPaperScissorsChoiceType type)
		=> RockPaperScissorsGame.Assets.First(x => x.ChoiceType == type);
}

/// <summary>
///     Constructs a new <see cref="RockPaperScissorsResponse" />.
/// </summary>
/// <param name="userChoiceAsset">The user choice asset.</param>
/// <param name="computerChoiceAsset">The computer choice asset.</param>
/// <param name="winType">The win type.</param>
public sealed class RockPaperScissorsResponse(RockPaperScissorsAsset userChoiceAsset, RockPaperScissorsAsset computerChoiceAsset, RockPaperScissorsWinType winType)
{
	/// <summary>
	///     Gets the computer's choice asset.
	/// </summary>
	public RockPaperScissorsAsset UserChoiceAsset { get; } = userChoiceAsset;

	/// <summary>
	///     Gets the computer's choice asset.
	/// </summary>
	public RockPaperScissorsAsset ComputerChoiceAsset { get; } = computerChoiceAsset;

	/// <summary>
	///     Gets the result.
	/// </summary>
	public RockPaperScissorsWinType WinType { get; } = winType;

	/// <summary>
	///     Gets the winner asset.
	/// </summary>
	public (string ImageUrl, string CreativeCommonsLicense) WinnerAsset
		=> this.WinType is RockPaperScissorsWinType.Computer or RockPaperScissorsWinType.Tie
			? (this.ComputerChoiceAsset.ImageUrl, this.ComputerChoiceAsset.CreativeCommonsLicense)
			: (this.UserChoiceAsset.ImageUrl, this.UserChoiceAsset.CreativeCommonsLicense);

	/// <summary>
	///     Tries to build and send a components V2 rock-paper-scissors message.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <returns>Whether the message was sent successfully.</returns>
	public async Task<bool> TryBuildV2RpsMessageAsync(InteractionContext ctx)
	{
		if (ctx.GuildId is not 1317206872763404478)
			return false;

		DiscordWebhookBuilder builder = new();
		builder.WithV2Components();
		DiscordTextDisplayComponent userChoiceComponent = new($"### {ctx.User.Mention} chooses {this.UserChoiceAsset.ChoiceType.ToString().InlineCode()}");
		DiscordSectionComponent userChoiceSection = new([userChoiceComponent]);
		userChoiceSection.WithThumbnailComponent(ctx.User.AvatarUrl);
		DiscordSeparatorComponent seperator1 = new(false, SeparatorSpacingSize.Small);
		DiscordTextDisplayComponent computerChoiceComponent = new($"### I choose {this.ComputerChoiceAsset.ChoiceType.ToString().InlineCode()}");
		DiscordSectionComponent computerChoiceSection = new([computerChoiceComponent]);
		computerChoiceSection.WithThumbnailComponent(ctx.Client.CurrentUser.AvatarUrl);
		DiscordSeparatorComponent seperator2 = new(true, SeparatorSpacingSize.Large);
		DiscordTextDisplayComponent resultTextComponent = new($"### {this}");
		DiscordSectionComponent resultSectionComponent = new([resultTextComponent]);
		resultSectionComponent.WithThumbnailComponent(this.WinnerAsset.ImageUrl, this.WinnerAsset.CreativeCommonsLicense);
		builder.AddComponents(new DiscordContainerComponent([userChoiceSection, seperator1, computerChoiceSection, seperator2, resultSectionComponent]));
		builder.WithAllowedMention(new UserMention(ctx.User));
		await ctx.EditResponseAsync(builder);
		return true;
	}

	/// <summary>
	///     Sends an old-style embed rock-paper-scissors message.
	/// </summary>
	/// <param name="ctx">The context.</param>
	public async Task SendOldStyleRpsMessageAsync(InteractionContext ctx)
	{
		var emb = new DiscordEmbedBuilder()
			.WithTitle("Rock Paper Scissors")
			.WithDescription($"{ctx.User.Mention} chooses {this.UserChoiceAsset.ChoiceType} and I choose {this.ComputerChoiceAsset.ChoiceType}.\n{this}")
			.WithThumbnail(this.WinnerAsset.ImageUrl)
			.WithFooter(this.WinnerAsset.CreativeCommonsLicense, "https://mirrors.creativecommons.org/presskit/icons/cc.xlarge.png");
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb));
	}

	/// <inheritdoc />
	public override string? ToString()
		=> this.WinType switch
		{
			RockPaperScissorsWinType.User => "You win :3",
			RockPaperScissorsWinType.Computer => "I win ^~^",
			RockPaperScissorsWinType.Tie => "It's a tie O.o",
			_ => null
		};
}

/// <summary>
///     Represents a rock paper scissors type.
/// </summary>
public enum RockPaperScissorsChoiceType
{
	/// <summary>
	///     The rock.
	/// </summary>
	Rock,

	/// <summary>
	///     The paper.
	/// </summary>
	Paper,

	/// <summary>
	///     The scissors.
	/// </summary>
	Scissors
}

/// <summary>
///     Represents a rock paper scissors win type.
/// </summary>
public enum RockPaperScissorsWinType
{
	/// <summary>
	///     The user wins.
	/// </summary>
	User,

	/// <summary>
	///     The computer wins.
	/// </summary>
	Computer,

	/// <summary>
	///     It's a tie.
	/// </summary>
	Tie
}
