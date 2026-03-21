# Dead Code / Live Code Map

Analysis date: 2026-03-21

Branch context:
- Current branch: `system-service`
- Base branch used for comparison: `master`
- There is no local `main` branch in this clone, so this map is based on `master...system-service`

This file is intentionally branch-aware.
It is not just “what looks old”, but “what changed relative to `master`, what got replaced, and what is currently live vs stranded.”

## Executive Read

The current branch is a real architectural migration branch.

Compared to `master`, it:

- replaces `MikuBot` with `HatsuneMikuBot`
- replaces the old monolithic command files with split command modules
- replaces a DB-driven music/guild state model with an in-memory `MusicSession` model
- moves old entities/utilities into explicit `Old/` folders
- keeps the old playlist/data model mostly as historical reference, but does not finish wiring the new playlist layer

So the repo now has three important states:

1. Live
   - used by the current branch runtime
2. Legacy reference
   - not used by the current branch runtime, but clearly preserved on purpose because it still contains implementation history
3. Stub / migration gap
   - intended to be live eventually, but currently hollowed out or placeholder-only in this branch

That third category is the spicy one.

## History Summary

The diff from `master...system-service` is huge:

- 103 files changed
- 10,596 insertions
- 5,443 deletions

The broad pattern is:

- old top-level command files deleted
- new split command files added
- old bot/runtime root deleted
- new bot/runtime root added
- old persistence-heavy music system removed from active use
- old entities/utilities rehomed under `Old/`

## Live Code

This is the stuff that is clearly part of the active runtime path on `system-service`.

### Bot bootstrap / runtime

- `MikuSharp/Program.cs`
- `MikuSharp/HatsuneMikuBot.cs`
- `MikuSharp/Entities/BotConfig.cs`
- `MikuSharp/GlobalUsings.cs`

Why this is live:

- `Program` instantiates `HatsuneMikuBot`
- command registration happens there
- shard/client/extensions are created there
- `RunAsync()` is the active runtime path

### Current command checks / command infrastructure

- `MikuSharp/Attributes/CommandAttributes.cs`

Why this is live:

- used by current command modules
- contains current deferral, playback, voice, and lavalink checks

### Current command modules

- `MikuSharp/Commands/AboutCommands.cs`
- `MikuSharp/Commands/ActionCommands.cs`
- `MikuSharp/Commands/DeveloperCommands.cs`
- `MikuSharp/Commands/DiscordUtilityCommands.cs`
- `MikuSharp/Commands/FunCommands.cs`
- `MikuSharp/Commands/MikuGuildCommands.cs`
- `MikuSharp/Commands/ModerationCommands.cs`
- `MikuSharp/Commands/NsfwCommands.cs`
- `MikuSharp/Commands/UtilityCommands.cs`
- `MikuSharp/Commands/WeebCommands.cs`
- `MikuSharp/Commands/Music/*`
- `MikuSharp/Commands/Playlist/*`

Why this is live:

- `HatsuneMikuBot.RegisterCommands()` registers these exact modules

Important nuance:

- `Playlist/*` is live in the sense that it is registered
- but much of it is functionally stubbed

So “registered live” is not always the same as “feature-complete live”.

### Current music model

- `MikuSharp/Entities/MusicSession.cs`
- `MikuSharp/Entities/MusicQueueEntry.cs`
- `MikuSharp/Utilities/MusicSessionExtensionMethods.cs`
- `MikuSharp/Utilities/LavalinkExtensionMethods.cs`
- `MikuSharp/Enums/Playing.cs`

Why this is live:

- current music commands operate through `MusicSession`
- guild music state is stored in `HatsuneMikuBot.MusicSessions`
- locking is handled by `MusicSessionLocks`

This is the new music spine of the branch.

### Current response and API helpers

- `MikuSharp/Utilities/DiscordExtensionMethods.cs`
- `MikuSharp/Utilities/WebExtensionMethods.cs`
- `MikuSharp/Utilities/Formatters.cs`
- `MikuSharp/Utilities/DiscordOptionProviders.cs`
- `MikuSharp/Utilities/NndExtensionMethods.cs`
- `MikuSharp/Utilities/Bilibili.cs`

Why this is live:

- directly referenced by current command files and current music flow

### Current event code

- `MikuSharp/Events/MikuGuildJoin.cs`

Why this is live:

- explicitly invoked from current `GuildMemberUpdated` event registration

### Current game/entity support

- `MikuSharp/Entities/Games/*`
- image DTOs under `MikuSharp/Entities/*`
- `NicoNicoNii/*`

Why this is live:

- current fun/image commands use them
- NicoNico support remains part of the current dependency graph

## Legacy Reference Code

This code looks intentionally preserved as migration history/reference, not actively executed.

### Old bot runtime

- `master:MikuSharp/MikuBot.cs`
  - deleted on this branch

What it used to do:

- own global bot runtime state
- own old guild/music state dictionaries
- partially wire CommandsNext, Lavalink, activity rotation, bot list stats
- register a much narrower active command surface than the current branch

Current status:

- fully replaced by `HatsuneMikuBot`
- dead in current branch runtime

Interpretation:

- this is not accidental dead code
- this is a completed replacement

### Old monolithic command files

Deleted from active path:

- `master:MikuSharp/Commands/About.cs`
- `master:MikuSharp/Commands/Action.cs`
- `master:MikuSharp/Commands/Developer.cs`
- `master:MikuSharp/Commands/Fun.cs`
- `master:MikuSharp/Commands/Music.cs`
- `master:MikuSharp/Commands/Playlist.cs`
- `master:MikuSharp/Commands/Utility.cs`
- `master:MikuSharp/Commands/Weeb.cs`
- `master:MikuSharp/Commands/NSFW.cs`

What happened:

- these were replaced by renamed/split modern command files
- the branch mostly preserved behavior by moving logic into better-grouped files

Current status:

- dead as files
- conceptually superseded, not abandoned

Interpretation:

- this is good dead code
- the branch made the codebase more navigable

### Old DB-centric music/persistence layer

Preserved under `Old/`:

- `MikuSharp/Entities/Old/Entry.cs`
- `MikuSharp/Entities/Old/Guild.cs`
- `MikuSharp/Entities/Old/MusicInstance.cs`
- `MikuSharp/Entities/Old/Playlist.cs`
- `MikuSharp/Entities/Old/PlaylistEntry.cs`
- `MikuSharp/Entities/Old/QueueEntry.cs`
- `MikuSharp/Entities/Old/TrackResult.cs`
- `MikuSharp/Utilities/Old/Database.cs`
- `MikuSharp/Utilities/Old/Music.cs`
- `MikuSharp/Utilities/Old/PlaylistDB.cs`
- `MikuSharp/Events/Old/VoiceChat.cs`

What these used to be on `master`:

- the actual live persistence and playback support
- queues stored in DB
- playlist CRUD backed by DB
- last-played history backed by DB
- `MusicInstance` carrying the old guild playback model
- `Lavalink.cs` driving end-of-track transitions against DB queue/history

Current status:

- dead from runtime
- preserved on purpose as implementation reference

Interpretation:

- this is a “museum wing”, not trash
- if someone has to reimplement playlists/history on the new branch, this is the source material

### Old standalone active files now effectively retired

Deleted and functionally replaced by either `Old/` or new abstractions:

- `MikuSharp/Utilities/Database.cs`
- `MikuSharp/Utilities/Music.cs`
- `MikuSharp/Utilities/PlaylistDB.cs`
- `MikuSharp/Events/Lavalink.cs`
- `MikuSharp/Events/VoiceChat.cs`
- `MikuSharp/Entities/MusicInstance.cs`
- `MikuSharp/Entities/Playlist.cs`
- `MikuSharp/Entities/PlaylistEntry.cs`
- `MikuSharp/Entities/QueueEntry.cs`
- `MikuSharp/Entities/TrackResult.cs`
- `MikuSharp/Entities/Guild.cs`
- `MikuSharp/Entities/Entry.cs`

Current status:

- dead in current branch
- their functional descendants are split between:
  - `Old/*`
  - new `MusicSession` stack
  - placeholder command modules

## Stub / Migration Gap Code

This is the most important section.

These things are live in registration or layout, but not truly implemented anymore.

### Playlist commands

Files:

- `MikuSharp/Commands/Playlist/PlaylistCommands.cs`
- `MikuSharp/Commands/Playlist/PlaylistCommands.CreateCommands.cs`
- `MikuSharp/Commands/Playlist/PlaylistCommands.ManageCommands.cs`
- `MikuSharp/Commands/Playlist/PlaylistCommands.SongCommands.cs`

Current behavior:

- commands mostly return placeholder text like:
  - `"Playlist from queue"`
  - `"Playlist play"`
  - `"Add song"`
  - `"Remove song"`

What existed on `master`:

- fully implemented playlist CRUD
- fixed playlists from YouTube/SoundCloud
- copying queue into playlists
- showing playlist contents
- delete/rename/clear
- playing playlist into queue
- song add/insert/move/remove

Interpretation:

- playlists are not dead conceptually
- they are actively mid-port
- the old implementation lives in `master` and now in `Old/`

Category:

- stubbed live surface

### Music info commands

File:

- `MikuSharp/Commands/Music/MusicCommands.InfoCommands.cs`

Current behavior:

- `now_playing` returns `"Now playing"`
- `last_played` returns `"Last played"`
- `last_playing` returns `"Last playing list"`

What existed on `master`:

- actual now-playing display
- actual last-played display
- actual paginated history list from DB-backed last-played table

Interpretation:

- another unfinished port

Category:

- stubbed live surface

### Queue display

File:

- `MikuSharp/Commands/Music/MusicCommands.QueueCommands.cs`

Current behavior:

- `show` returns `"Queue list"`

What existed on `master`:

- paginated queue view
- playback state info
- repeat/shuffle display

Interpretation:

- queue operations were partially ported
- queue visualization was not

Category:

- stubbed live surface

### Playlist and song autocomplete

File:

- `MikuSharp/Utilities/DiscordOptionProviders.cs`

Current behavior:

- `PlaylistProvider` returns `[]`
- `SongProvider` returns `[]`

What existed on `master`:

- playlist/song flows depended on actual DB-backed playlist content and autocomplete

Interpretation:

- the command surface was scaffolded before persistence was reconnected

Category:

- stubbed live support code

## Partial Replacement Areas

These are not dead, but their old and new designs differ a lot.

### Music architecture

On `master`:

- music state lived in `MikuBot.Guilds`
- each guild had a `Guild` object with a `MusicInstance`
- queue and last-played history were DB-backed
- end-of-track transitions lived in `Events/Lavalink.cs`

On `system-service`:

- music state lives in `ConcurrentDictionary<ulong, MusicSession>`
- queue is delegated to Lavalink built-in queue system
- state access is lock-guarded via `AsyncLock`
- status messages and playback state are updated in-memory

What this means:

- old queue/history logic is mostly gone from active runtime
- new playback core is cleaner
- but old persistence-backed features were not fully replaced

### Command organization

On `master`:

- big files with nested classes and mixed responsibility

On `system-service`:

- domain-split files
- partial classes for music and playlist
- separate utility command group

What this means:

- file-level dead code is low
- the branch mostly improved structure

## Truly Dead vs “Keep For Reference”

### Safe to call truly dead

These are gone with no real reason to treat them as active:

- `MikuSharp.sln`
  - replaced by `MikuSharp.slnx`
- `master:MikuSharp/MikuBot.cs`
  - runtime root replaced
- `master:MikuSharp/Commands/*.cs` monoliths
  - replaced by new command files
- deleted old DTO file names like:
  - `Img_Data.cs`
  - `KsoftSiRanImg.cs`
  - `MeekMoe.cs`
  - `Nekobot.cs`
  - `Nekos_Life.cs`

This is cleanup/renaming churn, not debt that still matters much.

### Should be kept in mind as reference, not deleted casually

- `MikuSharp/Entities/Old/*`
- `MikuSharp/Utilities/Old/*`
- `MikuSharp/Events/Old/VoiceChat.cs`
- `sql/*`

Reason:

- they preserve the only fully implemented version of playlists, queue persistence, and track history behavior

If you delete these before re-porting, you lose a lot of implementation memory.

## Feature Map

### Fully live on current branch

- bot startup / shard setup
- slash command registration
- developer eval tooling
- moderation basics
- Discord utility commands
- fun/image/weeb command families
- music join/leave
- music pause/resume/stop/seek/play
- music repeat/shuffle/volume
- queue skip/skip_to/remove/clear

### Live but degraded relative to `master`

- music queue visualization
- music history/info
- playlists

### Legacy-only on current branch

- DB queue persistence
- DB last-played persistence
- old guild-scoped music state model
- old `LavalinkTrackFinished` playback progression model

## What The History Says About Intent

The history makes the migration intent pretty obvious:

- `feat: prepare missing music & playlist commands`
- `extract discord utility to own group`
- `switch over to v2 components`
- `more docs`
- `group commands better & use capv2 if possible`
- `disable all music related things`

My read:

- this branch is deliberately modernizing structure and UI first
- then rebuilding behavior incrementally
- playlists/info/history didn’t make it all the way yet

So if something looks “dead” here, it often really means:

- “we replaced the architecture”
- or “we scaffolded the new endpoint but didn’t port the body yet”

## Recommended Practical Labels

If we wanted a clean internal vocabulary for this repo, I’d use:

### Live

- currently executed by `Program -> HatsuneMikuBot`

### Legacy

- not executed, but still authoritative reference for missing migrated behavior

### Stub

- registered command or helper with intentionally placeholder body

### Retired

- old file/type replaced cleanly, low need to preserve mentally

## My Current Map

### Live

- `MikuSharp/HatsuneMikuBot.cs`
- `MikuSharp/Attributes/CommandAttributes.cs`
- `MikuSharp/Commands/AboutCommands.cs`
- `MikuSharp/Commands/ActionCommands.cs`
- `MikuSharp/Commands/DeveloperCommands.cs`
- `MikuSharp/Commands/DiscordUtilityCommands.cs`
- `MikuSharp/Commands/FunCommands.cs`
- `MikuSharp/Commands/MikuGuildCommands.cs`
- `MikuSharp/Commands/ModerationCommands.cs`
- `MikuSharp/Commands/NsfwCommands.cs`
- `MikuSharp/Commands/UtilityCommands.cs`
- `MikuSharp/Commands/WeebCommands.cs`
- `MikuSharp/Commands/Music/*`
- `MikuSharp/Entities/MusicSession.cs`
- `MikuSharp/Utilities/MusicSessionExtensionMethods.cs`
- `MikuSharp/Utilities/DiscordExtensionMethods.cs`
- `MikuSharp/Utilities/WebExtensionMethods.cs`
- `NicoNicoNii/*`

### Legacy

- `MikuSharp/Entities/Old/*`
- `MikuSharp/Utilities/Old/*`
- `MikuSharp/Events/Old/VoiceChat.cs`
- `sql/*`

### Stub

- `MikuSharp/Commands/Playlist/*`
- `MikuSharp/Commands/Music/MusicCommands.InfoCommands.cs`
- `MikuSharp/Commands/Music/MusicCommands.QueueCommands.cs` for `show`
- `MikuSharp/Utilities/DiscordOptionProviders.cs` for playlist/song autocomplete

### Retired

- `master:MikuSharp/MikuBot.cs`
- `master:MikuSharp/Commands/About.cs`
- `master:MikuSharp/Commands/Action.cs`
- `master:MikuSharp/Commands/Developer.cs`
- `master:MikuSharp/Commands/Fun.cs`
- `master:MikuSharp/Commands/Music.cs`
- `master:MikuSharp/Commands/Playlist.cs`
- `master:MikuSharp/Commands/Utility.cs`
- `master:MikuSharp/Commands/Weeb.cs`
- `master:MikuSharp/Commands/NSFW.cs`
- `MikuSharp.sln`

## Final Take

The branch history changes the diagnosis a lot:

- before looking at history, some files looked like random dead leftovers
- after looking at history, they read more like a deliberate port-in-progress

The most important conclusion is:

playlist/history code is not dead because the project stopped caring about it.

It is “dead in the new runtime because the old implementation was DB-centric and the new music architecture replaced the foundation before fully porting all behaviors.”

That is a very different kind of dead.

If you want, next I can turn this into:

- a “porting checklist from `master` to current branch”
- a “delete now / keep for reference” cleanup plan
- or a “missing feature parity” doc for music + playlists
