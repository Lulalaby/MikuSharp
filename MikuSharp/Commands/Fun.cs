using HeyRed.Mime;

using MikuSharp.Attributes;
using MikuSharp.Entities;
using MikuSharp.Utilities;

namespace MikuSharp.Commands;

[SlashCommandGroup("fun", "Fun commands", allowedContexts: [InteractionContextType.Guild, InteractionContextType.PrivateChannel], integrationTypes: [ApplicationCommandIntegrationTypes.GuildInstall, ApplicationCommandIntegrationTypes.UserInstall]), DeferResponseAsync]
internal class Fun : ApplicationCommandsModule
{
	[SlashCommand("8ball", "Yes? No? Maybe?")]
	public static async Task EightBallAsync(InteractionContext ctx, [Option("text", "Text to modify")] string text)
	{
		var responses = new[]
		{
			"It is certain.",
			"It is decidedly so.",
			"Without a doubt.",
			"Yes - definitely.",
			"You may rely on it.",
			"As I see it, yes.",
			"Most likely.",
			"Outlook good.",
			"Yes.",
			"Signs point to yes.",
			"Absolutely!",
			"Of course!",
			"No doubt about it.",
			"The universe says yes.",
			"You got it!",
			"Definitely!",
			"All signs point to yes.",
			"The answer is crystal clear.",
			"Yes, in due time.",
			"The stars align in your favor.",
			"Reply hazy, try again.",
			"Ask again later.",
			"Better not tell you now.",
			"Cannot predict now.",
			"Concentrate and ask again.",
			"Maybe, maybe not.",
			"Uncertain, check back later.",
			"Hard to say.",
			"Try flipping a coin.",
			"Your guess is as good as mine.",
			"The future is unclear.",
			"I can't say for sure.",
			"It's a mystery.",
			"Only time will tell.",
			"50/50 chance.",
			"Don't count on it.",
			"My reply is no.",
			"My sources say no.",
			"Outlook not so good.",
			"Very doubtful.",
			"No.",
			"Absolutely not.",
			"I wouldn’t bet on it.",
			"No way!",
			"Highly unlikely.",
			"Not in a million years.",
			"Doubtful at best.",
			"The odds aren’t in your favor.",
			"The universe says no.",
			"Nope."
		};

		var chosenAnswer = responses[new Random().Next(0, responses.Length)];
		if (ctx.GuildId is 1317206872763404478)
		{
			DiscordTextDisplayComponent question = new($"### Question\n{text}");
			DiscordSectionComponent questionComponent = new([question]);
			questionComponent.WithThumbnailComponent(ctx.User.AvatarUrl);
			DiscordSeparatorComponent seperator = new(false, SeparatorSpacingSize.Small);
			DiscordTextDisplayComponent answer = new($"### Answer\n{chosenAnswer}");
			DiscordSectionComponent answerComponent = new([answer]);
			answerComponent.WithThumbnailComponent(ctx.Client.CurrentUser.AvatarUrl);
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithV2Components().AddComponents(new DiscordContainerComponent([questionComponent]), seperator, new DiscordContainerComponent([answerComponent])));
			return;
		}

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"> {text}\n\n{chosenAnswer}"));
	}

	[SlashCommand("coinflip", "Flip a coin lol")]
	public static async Task CoinflipAsync(InteractionContext ctx)
	{
		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new());
		var flip = new[] { $"Heads {DiscordEmoji.FromName(ctx.Client, ":arrow_up_small:")}", $"Tails {DiscordEmoji.FromName(ctx.Client, ":arrow_down_small:")}" };
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(flip[new Random().Next(0, flip.Length)]));
	}

	[SlashCommand("rps", "Play rock paper scissors!")]
	public static async Task RpsAsync(InteractionContext ctx, [Option("rps", "Your rock paper scissor choice")] string rps)
	{
		var rock = new[] { $"Rock {DiscordEmoji.FromName(ctx.Client, ":black_circle:")}", $"Paper {DiscordEmoji.FromName(ctx.Client, ":pencil:")}", $"Scissors {DiscordEmoji.FromName(ctx.Client, ":scissors:")}" };
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} choose {rps}!\n\nI choose {rock[new Random().Next(0, rock.Length)]}"));
	}

	[SlashCommandGroup("random_images", "Random images")]
	public class RandomImages : ApplicationCommandsModule
	{
		[SlashCommand("cat", "Get a random cat image!")]
		public static async Task CatAsync(InteractionContext ctx)
		{
			var imgUrl = await ctx.Client.RestClient.GetNekosLifeAsync("https://nekos.life/api/v2/img/meow");
			if (imgUrl is null)
			{
				await ctx.EditResponseAsync("Something went wrong while fetching the image.");
				return;
			}

			DiscordWebhookBuilder builder = new();
			builder.AddFile($"image.{imgUrl.Filetype}", imgUrl.Data);
			builder.AddEmbed(imgUrl.Embed);
			await ctx.EditResponseAsync(builder);
		}

		[SlashCommand("dog", "Random Dog Image")]
		public static async Task DogAsync(InteractionContext ctx)
		{
			var dc = JsonConvert.DeserializeObject<DogCeo>(await ctx.Client.RestClient.GetStringAsync("https://dog.ceo/api/breeds/image/random"));
			if (dc is null)
			{
				await ctx.EditResponseAsync("Something went wrong while fetching the image.");
				return;
			}

			var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(dc.Message.ResizeLink()));
			var em = new DiscordEmbedBuilder();
			em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
			em.WithFooter("by dog.ceo", "https://dog.ceo/img/favicon.png");
			em.WithDescription($"[Full Image]({dc.Message})");

			DiscordWebhookBuilder builder = new();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddEmbed(em.Build());
			await ctx.EditResponseAsync(builder);
		}

		[SlashCommand("duck", "Random duck image")]
		public static async Task DuckAsync(InteractionContext ctx)
		{
			var dc = JsonConvert.DeserializeObject<RandomD>(await ctx.Client.RestClient.GetStringAsync("https://random-d.uk/api/v1/random"));
			if (dc is null)
			{
				await ctx.EditResponseAsync("Something went wrong while fetching the image.");
				return;
			}

			var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(dc.Message.ResizeLink()));
			var em = new DiscordEmbedBuilder();
			em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
			em.WithFooter("by random-d.uk", "https://random-d.uk/favicon.png");
			em.WithDescription($"[Full Image]({dc.Message})");

			var builder = new DiscordWebhookBuilder();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			builder.AddEmbed(em.Build());
			await ctx.EditResponseAsync(builder);
		}

		[SlashCommand("lizard", "Get a random lizard image")]
		public static async Task LizardAsync(InteractionContext ctx)
		{
			var get = await ctx.Client.RestClient.GetNekosLifeAsync("https://nekos.life/api/lizard");
			if (get is null)
			{
				await ctx.EditResponseAsync("Something went wrong while fetching the image.");
				return;
			}

			var img = new MemoryStream(await ctx.Client.RestClient.GetByteArrayAsync(get.Url.ResizeLink()));

			DiscordWebhookBuilder builder = new();
			builder.AddFile($"image.{MimeGuesser.GuessExtension(img)}", img);
			await ctx.EditResponseAsync(builder);
		}
	}

	[SlashCommandGroup("memes", "Meme commands (powered by nekos.life and nekobot.xyz)")]
	public class Memes : ApplicationCommandsModule
	{
		[SlashCommand("stickbug", "Get stickbugged!")]
		public static async Task StickbugAsync(InteractionContext ctx, [Option("user", "User to stickbug")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("stickbug", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("trash", "Trash waifu image generator.")]
		public static async Task TrashAsync(InteractionContext ctx, [Option("user", "User to trash")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("trash", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("magik", "Magikify an image!")]
		public static async Task MagikAsync(InteractionContext ctx, [Option("user", "User to magikify")] DiscordUser? user = null, [Option("intensity", "Magik intensity (0-10)")] int intensity = 5)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("magik", new()
			{
				{ "image", ctx.GetGuildAvatarIfPossible(user) },
				{ "intensity", intensity.ToString() }
			});
		}

		[SlashCommand("phcomment", "Make a PH comment!")]
		public static async Task PhCommentAsync(InteractionContext ctx, [Option("user", "User to write as")] DiscordUser user, [Option("text", "Text to comment.")] string text)
		{
			await ctx.GenerateNekobotImageAsync("phcomment", new()
			{
				{ "image", ctx.GetGuildAvatarIfPossible(user) },
				{ "text", text },
				{ "username", ctx.GetGuildOrGlobalDisplayNameIfPossible(user) }
			});
		}

		[SlashCommand("blurpify", "Blurpify an image!")]
		public static async Task BlurpifyAsync(InteractionContext ctx, [Option("user", "User to blurpify")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("blurpify", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("deepfry", "Deepfry an image!")]
		public static async Task DeepfryAsync(InteractionContext ctx, [Option("user", "User to deepfry")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("deepfry", new()
			{
				{ "image", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("kidnap", "Kidnap a user!")]
		public static async Task KidnapAsync(InteractionContext ctx, [Option("user", "User to kidnap")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("kidnap", new()
			{
				{ "image", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("tweet", "Generate a fake tweet!")]
		public static async Task TweetAsync(InteractionContext ctx, [Option("text", "Text of the tweet")] string text, [Option("user", "User for tweet")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("tweet", new()
			{
				{ "username", ctx.GetGuildOrGlobalDisplayNameIfPossible(user) },
				{ "text", text }
			});
		}

		[SlashCommand("trap", "Trap someone!")]
		public static async Task TrapAsync(InteractionContext ctx, [Option("user", "User to trap")] DiscordUser user, [Option("author", "User trapping them")] DiscordUser? author = null)
		{
			author ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("trap", new()
			{
				{ "name", ctx.GetGuildOrGlobalDisplayNameIfPossible(user) },
				{ "author", ctx.GetGuildOrGlobalDisplayNameIfPossible(author) },
				{ "image", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("iphonex", "Insert an image into an iPhone X frame.")]
		public static async Task IPhoneXAsync(InteractionContext ctx, [Option("user", "User to insert")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("iphonex", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("lolice", "Call the lolice!")]
		public static async Task LoliceAsync(InteractionContext ctx, [Option("user", "User to call lolice on")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("lolice", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("kannagen", "Kanna says something!")]
		public static async Task KannaGenAsync(InteractionContext ctx, [Option("text", "Text for Kanna")] string text)
		{
			await ctx.GenerateNekobotImageAsync("kannagen", new()
			{
				{ "text", text }
			});
		}

		[SlashCommand("changemymind", "Change my mind meme generator.")]
		public static async Task ChangeMyMindAsync(InteractionContext ctx, [Option("text", "Change my mind text")] string text)
		{
			await ctx.GenerateNekobotImageAsync("changemymind", new()
			{
				{ "text", text }
			});
		}

		[SlashCommand("whowouldwin", "Who would win?")]
		public static async Task WhoWouldWinAsync(InteractionContext ctx, [Option("user1", "First user")] DiscordUser user1, [Option("user2", "Second user")] DiscordUser user2)
		{
			await ctx.GenerateNekobotImageAsync("whowouldwin", new()
			{
				{ "user1", ctx.GetGuildAvatarIfPossible(user1) },
				{ "user2", ctx.GetGuildAvatarIfPossible(user2) }
			});
		}

		[SlashCommand("captcha", "Generate a fake captcha!")]
		public static async Task CaptchaAsync(InteractionContext ctx, [Option("user", "User to display (their name)")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("captcha", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(ctx.User) },
				{ "username", ctx.GetGuildOrGlobalDisplayNameIfPossible(user) }
			});
		}

		[SlashCommand("ship", "Ship two users!")]
		public static async Task ShipAsync(InteractionContext ctx, [Option("user1", "First user")] DiscordUser user1, [Option("user2", "Second user")] DiscordUser user2)
		{
			await ctx.GenerateNekobotImageAsync("ship", new()
			{
				{ "user1", ctx.GetGuildAvatarIfPossible(user1) },
				{ "user2", ctx.GetGuildAvatarIfPossible(user2) }
			});
		}

		[SlashCommand("baguette", "Baguette someone!")]
		public static async Task BaguetteAsync(InteractionContext ctx, [Option("user", "User to baguette")] DiscordUser? user = null)
		{
			user ??= ctx.User;
			await ctx.GenerateNekobotImageAsync("baguette", new()
			{
				{ "url", ctx.GetGuildAvatarIfPossible(user) }
			});
		}

		[SlashCommand("clyde", "Say something as clyde bot")]
		public static async Task ClydeAsync(InteractionContext ctx, [Option("text", "Text to modify")] string text)
		{
			await ctx.GenerateNekobotImageAsync("clyde", new()
			{
				{ "text", text }
			});
		}
	}
}
