using MikuSharp.Utilities;

namespace MikuSharp.Entities.Games;

/// <summary>
///     Represents the 8ball game.
/// </summary>
public sealed class EightBallGame
{
	/// <summary>
	///     Gets a random response from the 8ball.
	/// </summary>
	/// <param name="userQuestion">The user question.</param>
	/// <returns>The response.</returns>
	public EightBallGame(string userQuestion)
	{
		Random random = new();
		var responseList = Responses[random.Next(Responses.Count)].ToList();
		this.ChosenAnswer = responseList[random.Next(responseList.Count)];
		this.UserQuestion = userQuestion;
	}

	/// <summary>
	///     A list of negative responses that the 8ball can give.
	/// </summary>
	public static IEnumerable<string> NegativeResponses { get; } = ["Don't count on it.", "My reply is no.", "My sources say no.", "Outlook not so good.", "Very doubtful.", "No.", "Absolutely not.", "I wouldn’t bet on it.", "No way!", "Highly unlikely.", "Not in a million years.", "Doubtful at best.", "The odds aren’t in your favor.", "The universe says no.", "Nope."];

	/// <summary>
	///     A list of positive responses that the 8ball can give.
	/// </summary>
	public static IEnumerable<string> PositiveResponses { get; } = ["It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.", "As I see it, yes.", "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.", "Absolutely!", "Of course!", "No doubt about it.", "The universe says yes.", "You got it!", "Definitely!", "All signs point to yes.", "The answer is crystal clear.", "Yes, in due time.", "The stars align in your favor."];

	/// <summary>
	///     A list of neutral responses that the 8ball can give.
	/// </summary>
	public static IEnumerable<string> NeutralResponse { get; } = ["Reply hazy, try again.", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.", "Maybe, maybe not.", "Uncertain, check back later.", "Hard to say.", "Try flipping a coin.", "Your guess is as good as mine.", "The future is unclear.", "I can't say for sure.", "It's a mystery.", "Only time will tell.", "50/50 chance."];

	/// <summary>
	///     A list of all the responses that the 8ball can give.
	/// </summary>
	public static IReadOnlyList<IEnumerable<string>> Responses { get; } = [NegativeResponses, PositiveResponses, NeutralResponse];

	/// <summary>
	///     Gets the chosen answer.
	/// </summary>
	public string ChosenAnswer { get; }

	/// <summary>
	///     Gets the user question.
	/// </summary>
	public string UserQuestion { get; }

	/// <summary>
	///     Tries to build and send a components V2 8ball message.
	/// </summary>
	/// <param name="ctx">The context.</param>
	/// <returns>Whether the message was sent successfully.</returns>
	public async Task Send8BallMessageAsync(InteractionContext ctx)
	{
		DiscordTextDisplayComponent question = new($"### Question\n{this.UserQuestion}");
		DiscordSectionComponent questionComponent = new([DiscordExtensionMethods.EmptyComponent, question]);
		questionComponent.WithThumbnailComponent(ctx.User.AvatarUrl);
		DiscordSeparatorComponent seperator = new(false, SeparatorSpacingSize.Small);
		DiscordTextDisplayComponent answer = new($"### Answer\n{this.ChosenAnswer}");
		DiscordSectionComponent answerComponent = new([DiscordExtensionMethods.EmptyComponent, answer]);
		answerComponent.WithThumbnailComponent(ctx.Client.CurrentUser.AvatarUrl);
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithV2Components().AddComponents(new DiscordContainerComponent([new DiscordTextDisplayComponent("## 8Ball"), new DiscordSeparatorComponent(true, SeparatorSpacingSize.Large), questionComponent, seperator, answerComponent])));
	}
}
