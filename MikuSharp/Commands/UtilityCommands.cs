using Kitsu.Anime;
using Kitsu.Manga;

using MikuSharp.Attributes;

namespace MikuSharp.Commands;

[SlashCommandGroup("utility", "Utilities")]
internal class UtilityCommands : ApplicationCommandsModule
{
	[SlashCommandGroup("am", "Anime & Mange"), DeferResponseAsync]
	internal class AnimeMangaUtility : ApplicationCommandsModule
	{
		[SlashCommand("anime_search", "Search for an anime")]
		public static async Task SearchAnimeAsync(InteractionContext ctx, [Option("search_query", "Search query")] string searchQuery)
		{
			try
			{
				var ine = ctx.Client.GetInteractivity();
				var a = await Anime.GetAnimeAsync(searchQuery);
				var emb = new DiscordEmbedBuilder();
				List<DiscordEmbedBuilder> res = [];
				List<Page> ress = [];

				foreach (var aa in a.Data)
				{
					emb.WithColor(new(0212255));
					emb.WithTitle(aa.Attributes.Titles.EnJp);
					if (aa.Attributes.Synopsis.Length is not 0)
						emb.WithDescription(aa.Attributes.Synopsis);
					if (aa.Attributes.Subtype.Length is not 0)
						emb.AddField(new("Type", $"{aa.Attributes.Subtype}", true));
					if (aa.Attributes.EpisodeCount is not null)
						emb.AddField(new("Episodes", $"{aa.Attributes.EpisodeCount}", true));
					if (aa.Attributes.EpisodeLength is not null)
						emb.AddField(new("Length", $"{aa.Attributes.EpisodeLength}", true));
					if (aa.Attributes.StartDate is not null)
						emb.AddField(new("Start Date", $"{aa.Attributes.StartDate}", true));
					if (aa.Attributes.EndDate is not null)
						emb.AddField(new("End Date", $"{aa.Attributes.EndDate}", true));
					if (aa.Attributes.AgeRating is not null)
						emb.AddField(new("Age Rating", $"{aa.Attributes.AgeRating}", true));
					if (aa.Attributes.AverageRating is not null)
						emb.AddField(new("Score", $"{aa.Attributes.AverageRating}", true));
					emb.AddField(new("NSFW", $"{aa.Attributes.Nsfw}", true));
					if (aa.Attributes.CoverImage?.Small is not null)
						emb.WithThumbnail(aa.Attributes.CoverImage.Small);
					res.Add(emb);
					emb = new();
				}

				res.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.Ordinal));
				var i = 1;

				foreach (var aa in res)
				{
					aa.WithFooter($"via Kitsu.io -- Page {i}/{a.Data.Count}", "https://kitsu.io/kitsu-256-ed442f7567271af715884ca3080e8240.png");
					ress.Add(new(embed: aa));
					i++;
				}

				await ine.SendPaginatedResponseAsync(ctx.Interaction, true, ctx.Guild is not null, ctx.User, ress, behaviour: PaginationBehaviour.WrapAround, deletion: ButtonPaginationBehavior.Disable);
			}
			catch (Exception ex)
			{
				ctx.Client.Logger.LogError("{ex}", ex.Message);
				ctx.Client.Logger.LogError("{ex}", ex.StackTrace);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No Anime found!"));
			}
		}

		[SlashCommand("manga_search", "Search for an manga")]
		public static async Task SearchMangaAsync(InteractionContext ctx, [Option("search_query", "Search query")] string searchQuery)
		{
			try
			{
				var ine = ctx.Client.GetInteractivity();
				var a = await Manga.GetMangaAsync(searchQuery);
				var emb = new DiscordEmbedBuilder();
				List<DiscordEmbedBuilder> res = [];
				List<Page> ress = [];

				foreach (var aa in a.Data)
				{
					emb.WithColor(new(0212255));
					emb.WithTitle(aa.Attributes.Titles.EnJp);
					if (aa.Attributes.Synopsis is not null)
						emb.WithDescription(aa.Attributes.Synopsis);
					if (aa.Attributes.Subtype is not null)
						emb.AddField(new("Type", $"{aa.Attributes.Subtype}", true));
					if (aa.Attributes.StartDate is not null)
						emb.AddField(new("Start Date", $"{aa.Attributes.StartDate}", true));
					if (aa.Attributes.EndDate is not null)
						emb.AddField(new("End Date", $"{aa.Attributes.EndDate}", true));
					if (aa.Attributes.AgeRating is not null)
						emb.AddField(new("Age Rating", $"{aa.Attributes.AgeRating}", true));
					if (aa.Attributes.AverageRating is not null)
						emb.AddField(new("Score", $"{aa.Attributes.AverageRating}", true));
					if (aa.Attributes.CoverImage?.Small is not null)
						emb.WithThumbnail(aa.Attributes.CoverImage.Small);
					emb.WithFooter("via Kitsu.io", "https://kitsu.io/kitsu-256-ed442f7567271af715884ca3080e8240.png");
					res.Add(emb);
					emb = new();
				}

				res.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.Ordinal));
				var i = 1;

				foreach (var aa in res)
				{
					aa.WithFooter($"via Kitsu.io -- Page {i}/{a.Data.Count}", "https://kitsu.io/kitsu-256-ed442f7567271af715884ca3080e8240.png");
					ress.Add(new(embed: aa));
					i++;
				}

				await ine.SendPaginatedResponseAsync(ctx.Interaction, true, ctx.Guild is not null, ctx.User, ress, behaviour: PaginationBehaviour.WrapAround, deletion: ButtonPaginationBehavior.Disable);
			}
			catch (Exception ex)
			{
				ctx.Client.Logger.LogError("{ex}", ex.Message);
				ctx.Client.Logger.LogError("{ex}", ex.StackTrace);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No Manga found!"));
			}
		}
	}
}
