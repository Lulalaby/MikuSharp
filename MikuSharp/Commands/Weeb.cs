﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using HeyRed.Mime;
using MikuSharp.Utilities;
using System.Net.Http;

namespace MikuSharp.Commands
{
    class Weeb : BaseCommandModule
    {
        [Command("awooify")]
        [Priority(2)]
        [Description("Awooify your or someones avatar!")]
        public async Task Awooify(CommandContext ctx, DiscordMember member = null)
        {
            var c = new HttpClient();
            string avartURL = ctx.Member.AvatarUrl;
            if (member != null)
            {
                avartURL = member.AvatarUrl;
            }
            var e = JsonConvert.DeserializeObject<Entities.NekoBot>(await c.GetStringAsync($"https://nekobot.xyz/api/imagegen?type=awooify&url={avartURL}"));
            var embed2 = new DiscordEmbedBuilder();
            embed2.WithImageUrl(e.message);
            await ctx.RespondAsync(embed: embed2.Build());
        }

        [Command("awooify")]
        [Priority(1)]
        public async Task Awooify(CommandContext ctx, string member)
        {
            var c = new HttpClient();
            var AvatarUser = ctx.Guild.Members.Where(x => x.Value.Username.ToLower().Contains(member) | x.Value.DisplayName.ToLower().Contains(member));
            var e = JsonConvert.DeserializeObject<Entities.NekoBot>(await c.GetStringAsync($"https://nekobot.xyz/api/imagegen?type=awooify&url={AvatarUser.First().Value.AvatarUrl}"));
            var embed2 = new DiscordEmbedBuilder();
            embed2.WithImageUrl(e.message);
            await ctx.RespondAsync(embed: embed2.Build());
        }

        [Command("ddlc")]
        [Description("Radon DDLC image")]
        public async Task DDLC(CommandContext ctx)
        {
            var c = new HttpClient();
            var e = JsonConvert.DeserializeObject<Entities.Derpy>(await c.GetStringAsync($"https://miku.derpyenterprises.org/ddlcjson"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(e.url)));
            var em = new DiscordEmbedBuilder();
            img.Position = 0;
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by Derpy API");
            em.WithDescription($"{e.url}");
            await ctx.RespondWithFileAsync(fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}", embed: em.Build());
        }

        [Command("diva")]
        [Description("Radnom PJD Loading image")]
        public async Task DivaPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/diva"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            Console.WriteLine(MimeGuesser.GuessExtension(img));
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("gumi")]
        [Description("Random Gumi image")]
        public async Task GumiPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/gumi"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("kaito")]
        [Description("Random Kaito image")]
        public async Task KaitoPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/kaito"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("k-on")]
        [Description("Random K-On gif")]
        public async Task K_On(CommandContext ctx)
        {
            var c = new HttpClient();
            var e = JsonConvert.DeserializeObject<Entities.Derpy>(await c.GetStringAsync($"https://miku.derpyenterprises.org/k_onjson"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(e.url)))
            {
                Position = 0
            };
            var em = new DiscordEmbedBuilder();
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by Derpy API");
            em.WithDescription($"{e.url}");
            await ctx.RespondWithFileAsync(fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}", embed: em.Build());
        }

        [Command("konosuba")]
        [Description("Random Konosuba image")]
        public async Task Konosuba(CommandContext ctx)
        {
            var c = new HttpClient();
            var e = JsonConvert.DeserializeObject<Entities.Derpy>(await c.GetStringAsync($"https://miku.derpyenterprises.org/konosubajson"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(e.url)))
            {
                Position = 0
            };
            var em = new DiscordEmbedBuilder();
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by Derpy API");
            em.WithDescription($"{e.url}");
            await ctx.RespondWithFileAsync(fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}", embed: em.Build());
        }

        [Command("len")]
        [Description("Random Len image")]
        public async Task KLenPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/len"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("lovelive")]
        [Description("Random Love Live gif")]
        public async Task LoveLive(CommandContext ctx)
        {
            var c = new HttpClient();
            var e = JsonConvert.DeserializeObject<Entities.Derpy>(await c.GetStringAsync($"https://miku.derpyenterprises.org/lovelivejson"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(e.url)))
            {
                Position = 0
            };
            var em = new DiscordEmbedBuilder();
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by Derpy API");
            em.WithDescription($"{e.url}");
            await ctx.RespondWithFileAsync(fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}", embed: em.Build());
        }

        [Command("luka")]
        [Description("Random Luka image")]
        public async Task LukaPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/luka"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("meiko")]
        [Description("Random Meiko image")]
        public async Task MeikoPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/meiko"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("miku")]
        [Description("Random Miku image")]
        public async Task HMikuPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/miku"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("neko")]
        [Description("Get a random neko image")]
        public async Task Cat(CommandContext ctx)
        {
            var c = new HttpClient();
            var ImgURL = await Web.GetNekos_Life("https://nekos.life/api/v2/img/neko");
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(ImgURL.Url)));
            var em = new DiscordEmbedBuilder();
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by nekos.life");
            await ctx.RespondWithFileAsync(embed: em.Build(), fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}");
        }

        [Command("rin")]
        [Description("Random Rin image")]
        public async Task KRinPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/rin"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }

        [Command("takagi")]
        [Description("Random Takagi image")]
        public async Task Takagi(CommandContext ctx)
        {
            var c = new HttpClient();
            var e = JsonConvert.DeserializeObject<Entities.Derpy>(await c.GetStringAsync($"https://miku.derpyenterprises.org/takagijson"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(e.url)))
            {
                Position = 0
            };
            var em = new DiscordEmbedBuilder();
            em.WithImageUrl($"attachment://image.{MimeGuesser.GuessExtension(img)}");
            em.WithFooter("by Derpy API");
            em.WithDescription($"[Full Image]({e.url})");
            await ctx.RespondWithFileAsync(fileData: img, fileName: $"image.{MimeGuesser.GuessExtension(img)}", embed: em.Build());
        }

        [Command("teto")]
        [Description("Random Teto image")]
        public async Task KTetoPic(CommandContext ctx)
        {
            var c = new HttpClient();
            var res = JsonConvert.DeserializeObject<Entities.MeekMoe>(await c.GetStringAsync($"https://api.meek.moe/teto"));
            var img = new MemoryStream(await c.GetByteArrayAsync(Other.resizeLink(res.url)))
            {
                Position = 0
            };
            var emim = new DiscordEmbedBuilder
            {
                Description = $"[Full Source Image Link]({res.url})",
                ImageUrl = $"attachment://image.{MimeGuesser.GuessExtension(img)}"
            };
            if (res.creator.Length != 0)
            {
                emim.AddField("Creator", res.creator);
            }
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{MimeGuesser.GuessExtension(img)}", fileData: img, embed: emim.Build());
        }
    }
}
