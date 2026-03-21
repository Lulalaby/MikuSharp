# MikuSharp Repo Analysis

Analysis date: 2026-03-21

Scope notes:
- `TranslatorGenerator` was intentionally ignored.
- I did not do a deep content review of `NsfwCommands.cs`; I only acknowledged that the module exists and how it is wired.
- This analysis is based on the current worktree, which already has local uncommitted changes.
- I treated DisCatSharp as source-available through `C:\Users\Lulalaby\codex-workspaces\dave-experimental\discatsharp-codex`, but this repo itself still primarily consumes it through NuGet unless the `DebugSrc` configuration is used.

## TL;DR

This repo is a Discord bot solution centered around `MikuSharp`, with a small helper library `NicoNicoNii` for NicoNico/NND interactions and a set of SQL dumps from an older persistence model.

The codebase feels like a bot that has been partially modernized:
- command registration and a lot of message rendering have been moved to newer DisCatSharp application commands and V2 components
- the music session model has been rebuilt around in-memory state plus Lavalink
- but several feature areas are still half-migrated, placeholder, or preserved as legacy artifacts

The strongest active area right now looks like:
- bot bootstrap / shard setup
- image and utility commands
- basic music join/playback/queue control
- developer tooling / eval

The weakest or most obviously unfinished area looks like:
- playlist commands
- music info commands
- playlist/song autocomplete data
- SQL integration in the current code path

So, blunt cat opinion: this repo is not “messy beyond saving” at all, but it is definitely in a transitional arc. It has a real modern direction, and also some spicy fossil layers still hanging around.

## High-Level Layout

### Root

- `MikuSharp/`
  - main bot project
- `NicoNicoNii/`
  - helper library for NicoNico session/login/watch-page parsing
- `sql/`
  - old PostgreSQL dumps for playlists, queues, and last-played songs
- `MikuSharp.slnx`
  - current solution file
- `README.md`
  - deployment/runtime notes, especially Lavalink and Linux dependencies

### Main bot project folders

- `Attributes/`
  - custom command/application checks like defer, voice requirements, playback requirements
- `Commands/`
  - slash commands, context menu commands, music/playlist partial command modules, plus legacy NSFW command module
- `Entities/`
  - config DTOs, API DTOs, music session state, small game models, old persisted entities
- `Enums/`
  - playback/repeat-related enums and service enums
- `Events/`
  - guild/member events and older voice event leftovers
- `Utilities/`
  - response helpers, web/API helpers, formatting, autocomplete providers, old database/music helpers

## Solution / Build Model

`MikuSharp.csproj` targets `net10.0` and has two DisCatSharp modes:

- normal mode:
  - consumes `DisCatSharp*` packages version `10.7.0` from NuGet
- `DebugSrc` mode:
  - swaps to direct `ProjectReference`s pointing at your local DisCatSharp source tree

That is actually a pretty nice setup for library-dev workflow. It matches what you told me and means no dumb NuGet cache archaeology is needed.

Other notable build details:

- lots of native/audio/tooling binaries are copied into output
  - `ffmpeg.exe`, `youtube-dl.exe`, `nnd.exe`, opus/sodium DLLs, ffmpeg DLLs
- nullable is enabled
- many nullability warnings are suppressed in debug/release
- `README.md` is packed into the package metadata

## Runtime Startup Flow

### Entry point

`Program.cs` is tiny and sync-blocks on async startup:

1. create `HatsuneMikuBot`
2. call `RegisterEventsAsync().Wait()`
3. register commands
4. `RunAsync().Wait()`

This is old-school but straightforward.

### `HatsuneMikuBot`

This is the real composition root. It:

- reads `config.json`
- deserializes `BotConfig`
- constructs a PostgreSQL connection string
- sets up Serilog file + console logging
- creates a `DiscordShardedClient`
- wires Interactivity
- wires Application Commands
- wires CommandsNext
- wires Lavalink

It also owns several global/static pieces of state:

- `WeebClient`
- `MusicSessions`
- `MusicSessionLocks`
- `MikuCancellationTokenSource`
- static `Config`
- static `ShardedClient`
- static `DiscordBotListApi`

This makes the bot easy to access from anywhere, but it also tightly couples everything to global state.

### Event registration

`RegisterEventsAsync` hooks:

- global client error logging
- application command startup/ready logs
- slash/context-menu error handlers
- a guild-specific `GuildMemberUpdated` handler for the Miku support/home guild

The slash error handling is pragmatic:

- special-cases developer-only checks
- otherwise returns a generic ephemeral failure

### Command registration split

Two command systems are used on purpose:

- `ApplicationCommands`
  - main slash/context menu surface
- `CommandsNext`
  - only used for `NsfwCommands`

The comment literally says this is to keep NSFW stuff hidden. That split makes sense historically.

## Command Surface Overview

### General / info

- `AboutCommands`
  - donate, bot info, news-follow setup, feedback modal, ping, shard info, stats, support

These are reasonably feature-rich and already use modern component responses in several places.

### Utility / moderation / Discord helpers

- `DiscordUtilityCommands`
  - avatars, server info, user info, emoji and sticker pagination
- `UtilityCommands`
  - Kitsu-based anime/manga search
- `ModerationCommands`
  - invite toggles, ban, unban, kick, purge

This part feels active and usable.

### Fun / image / fandom-ish commands

- `ActionCommands`
  - hug, kiss, lick, pat, poke, slap, bite, nom, stare
- `FunCommands`
  - games
  - random animal images
  - meme/image-gen endpoints
- `WeebCommands`
  - character image pulls from `api.meek.moe`

These commands are mostly thin orchestration over external APIs plus some shared image/response helpers.

### Music

Split into partial classes:

- `MusicCommands.cs`
  - join, leave, test UI
- `InfoCommands`
  - now playing / last played / history
- `OptionsCommands`
  - repeat, shuffle, volume
- `PlaybackCommands`
  - pause, resume, stop, seek, play
- `QueueCommands`
  - show, skip, skip_to, remove, clear

This is one of the more important subsystems, and it has a very clear modern shape.

### Playlist

Also split into partial classes:

- create
- manage
- song

But most commands are currently placeholders that just respond with things like:

- `"Playlist from queue"`
- `"Playlist delete"`
- `"Playlist play"`

So the structure exists, but the feature is not fully implemented in the current code path.

### Developer-only

`DeveloperCommands.cs` is a juicy dev-tools module:

- context-menu eval
- fancy eval V2 UI flow
- delete-bot-message context menu
- shard test
- lavalink stats
- debug log retrieval
- monetization test commands

This is one of the most powerful files in the repo, both in good and dangerous ways.

## Music Subsystem Analysis

## What looks modern/good

### In-memory session model

`MusicSession` is a focused holder for:

- guild/channel binding
- lavalink session/player
- repeat mode
- playback state
- status message tracking

This is much cleaner than the older DB-heavy queue model.

### Per-guild locking

`MusicSessionExtensionMethods` uses `AsyncLock` per guild ID:

- session lookup/update is wrapped in helper methods
- commands mostly operate through `ExecuteWithMusicSessionAsync`

That is a good anti-race baseline for bot commands.

### Shared embed/status updates

The status-message approach is centralized:

- build status embed
- delete/replace previous status message
- keep music UI state coherent

This part feels intentionally designed, not accidental.

## What looks incomplete

### Playlist is not wired into the new music architecture yet

Biggest gap, easily.

Evidence:

- playlist commands are mostly placeholders
- playlist/song autocomplete providers return `[]`
- old DB helper lives in `Utilities/Old/PlaylistDB.cs`
- SQL dumps still exist, but current active code does not appear to use them

This screams “migration paused halfway”.

### Info commands are placeholders

`now_playing`, `last_played`, and `last_playing` currently just emit stub text.

### Queue display is incomplete

`queue show` currently only responds with `"Queue list"`.

## Behavioral notes

- music only works if Lavalink session exists and is connected
- `DISABLE_LAVALINK = true` is currently hardcoded in `HatsuneMikuBot`

That is extremely important:

- all music slash commands are guarded by `EnsureLavalinkSession`
- if Lavalink is disabled, the music surface is effectively unavailable

So right now the repo contains the music architecture, but the runtime default is “music off”.

That may be intentional for local/dev safety, but it is worth calling out loudly.

## Persistence / Database Story

### Current code

Active current code only clearly uses DB config construction in `BotConfig` / `HatsuneMikuBot`.

I did not find an active modern persistence layer for playlists or music history inside the current bot path.

### Legacy code

There is an older persistence model in:

- `Utilities/Old/Database.cs`
- `Utilities/Old/Music.cs`
- `Utilities/Old/PlaylistDB.cs`
- `Entities/Old/*`

`PlaylistDB.cs` is entirely commented out right now, which is honestly kind of funny in a very “we’ll rebuild this later” anime-timeskip way.

### SQL folder

`sql/public/*.sql` contains schema dumps and huge data dumps:

- `playlists.sql`
- `playlistentries.sql`
- `queues.sql`
- `lastplayedsongs.sql`

Observations:

- these are not clean migrations
- they are dump-style files with live-looking historical data
- they include user-generated content and track history
- they are massive and not really source-code ergonomics friendly

The SQL folder is more “archival data export / historical reference” than “current migration system”.

## External Integrations

This bot talks to a lot of services.

### Discord / bot infra

- DisCatSharp
- Application Commands
- CommandsNext
- Interactivity
- Lavalink
- DiscordBotsList API

### Media / fandom / image APIs

- `weeb.net`
- `nekos.life`
- `nekobot.xyz`
- `api.meek.moe`
- `dog.ceo`
- `random-d.uk`

### Music / video / metadata

- Lavalink source loading
- YouTube API packages
- YoutubeExplode
- `NYoutubeDL`
- NicoNico via `NicoNicoNii`

### Misc utility APIs/libraries

- Kitsu
- AngleSharp
- Mime guessing
- Serilog
- Roslyn scripting

This repo is very integration-heavy. That means a lot of “business logic” here is really orchestration and response formatting.

## NicoNicoNii Library

This subproject is small and focused.

### `NndClient`

Handles:

- login
- cookie/session storage
- session validity check
- logout

### `NicoVideoClient`

Handles:

- watch page fetch
- parsing `js-initial-watch-data`
- initializing anonymous/non-member watch session
- creating HTTP/HLS session requests for video APIs

This library is cleanly separated from the bot project and is one of the more cohesive parts of the repo. Nice little utility library, honestly.

## Code Quality / Design Observations

## Strengths

### 1. Clear feature grouping

The repo is easy to navigate:

- commands by domain
- helpers in utilities
- DTOs/entities grouped sensibly

### 2. Modern DisCatSharp usage is visible

You’re clearly using newer command metadata and newer component models, especially in:

- donate/about flows
- music UI test
- image response helpers

### 3. Good helper extraction for repeated response patterns

Examples:

- `DiscordExtensionMethods`
- `WebExtensionMethods`
- `MusicSessionExtensionMethods`
- command check attributes

### 4. Practical per-guild locking for music state

That’s one of the better engineering choices in the repo.

## Weaknesses / debt

### 1. Global static state everywhere

`HatsuneMikuBot` is effectively a service locator + singleton bag.

Tradeoff:

- easy access
- low ceremony
- but poor testability, high coupling, and hidden runtime dependencies

### 2. Mixed maturity levels

Some modules are polished, some are placeholder, some are undead legacy.

That makes the codebase feel inconsistent even when individual pieces are fine.

### 3. Sync-over-async startup

The use of `.Wait()` in `Program.cs` is old and unnecessary now.
Not instantly fatal, but definitely not my favorite.

### 4. Heavy warning suppression

The project enables nullable, then suppresses a lot of nullable warnings.

That usually means:

- the team wants modern typing benefits
- but the codebase is not fully clean yet

### 5. Some blocking / sync calls inside async paths

Example:

- `TryGetWeebNetImage` uses `.Result` on `GetByteArrayAsync`

That is a small but real smell.

### 6. Hardcoded IDs / environment assumptions

There are lots of baked-in constants:

- guild IDs
- channel/forum IDs
- emoji IDs
- developer user ID
- SKU IDs

That is normal for a real bot, but it also means portability is low without config extraction.

### 7. SQL dumps contain historical user data

From a repo hygiene standpoint, this is the biggest “yikes but understandable” thing.

## Security / Risk Notes

### Developer eval is powerful

The Roslyn eval commands can run arbitrary C# against live bot context.

This is okay if and only if:

- team-member checks are correct
- team membership is tightly controlled

If that guard ever regresses, it becomes instant god-mode.

### Config handling is file-based and local

`config.json` is read directly from disk.
Simple and fine, but:

- no validation layer beyond null checks
- secrets are expected locally

### Large archived SQL data in repo

Potential risk areas:

- user data exposure
- historical content in version control
- noise and repo bloat

### Music toolchain is operationally messy by nature

Bundled binaries + external download tooling + remote APIs + Lavalink means:

- lots of moving pieces
- easy breakage from upstream changes

That is not a criticism of your code specifically; it is just the cursed genre of music bots.

## “Legacy vs Active” Read

### Looks active / intended

- `HatsuneMikuBot`
- command modules except playlist/info placeholders
- music session helpers
- web/image helpers
- Discord V2 component responses
- `NicoNicoNii`

### Looks legacy / partially retired

- `Utilities/Old/*`
- `Entities/Old/*`
- SQL dump files
- older persistence model
- CommandsNext NSFW split as a historical compatibility choice

### Looks mid-migration

- playlist subsystem
- music info/history
- queue display
- autocomplete for playlist/song

## Recommended Mental Model For Future Work

If I were working in this repo more, I’d mentally split it into 4 zones:

### 1. Core bot runtime

Startup, sharding, logging, config, command registration.

### 2. Stable command/UI layer

About, moderation, Discord utilities, image commands, action commands.

### 3. Music rewrite layer

Current `MusicSession` + Lavalink + queue/playback logic.

### 4. Legacy persistence layer waiting for replacement

Old DB helpers, SQL dumps, placeholder playlist commands.

That mental split makes the repo way easier to reason about.

## What I’d prioritize next if we were refining the repo

### Highest value

1. Finish the playlist migration
   - implement active persistence layer
   - wire autocomplete providers
   - replace placeholder command bodies

2. Decide whether music is meant to be on by default
   - `DISABLE_LAVALINK = true` is too important to be accidental

3. Replace the SQL dump folder with real migrations or archive it elsewhere
   - especially because it contains historical data

### Medium value

4. Move hardcoded guild/channel/etc. IDs into config
5. Remove obvious sync blocking in async flows
6. Reduce nullable warning suppressions over time

### Nice cleanup

7. Convert old commented-out code into either:
   - deleted history
   - or a proper `docs/legacy-notes.md`

8. Separate “finished” vs “stubbed” commands more clearly in docs

## Final Take

This repo is very workable.

The architecture is not “clean enterprise temple code”, but it has a strong real-world bot-dev shape:

- fast iteration
- service integrations everywhere
- some hardcoded operational facts
- some old baggage
- some genuinely nice modernization work

The most important truth is probably this:

the repo is not suffering from a lack of structure, it is suffering from an unfinished migration.

That is actually good news, because unfinished migrations are way easier to fix than fundamentally directionless codebases.

If you want, next I can turn this into either:

- a “hotspots / fix roadmap” doc
- a “what is dead code vs what is live code” checklist
- or a proper architecture map file with per-folder notes
