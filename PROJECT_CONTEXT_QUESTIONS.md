# Project Context Questions

Use this however you want:
- answer inline under each question
- delete questions you don't care about
- add your own sections
- write in chaos mode if that feels better

You do not need to answer everything.
Even partial answers will help a lot for future analysis and coding work.

---

## Project Direction

### What do you consider the bot's core identity now?
Answer:

### Which features are still important to you personally?
Answer:

### Which features are only kept because users still expect them?
Answer:

### Which features would you happily kill if nobody complained?
Answer:

### What is your ideal future state for MikuSharp in one sentence?
Answer:

### Do you see this as a hobby project, a production service, a legacy system to stabilize, or a thing you still want to seriously evolve?
Answer:

---

## Current Status

### Is the production bot still running this repo, or a fork / older branch / patched deployment variant?
Answer:

### Which branch is closest to prod right now?
Answer:

### Is `system-service` meant to become the new mainline, or is it more experimental?
Answer:

### What parts of the current branch are known-good in prod?
Answer:

### What parts are definitely unfinished and not deployed?
Answer:

### Are there local-only hotfixes or dirty prod patches that are not committed anywhere?
Answer:

---

## Architecture History

### What was the rough timeline of major eras of this bot?
Answer:

### When did you switch from old voice to Lavalink?
Answer:

### What broke hardest during that migration?
Answer:

### Was the move to the current `MusicSession` model a response to bugs, performance, maintainability, or all of the above?
Answer:

### Did sharding come in because Discord required it, because of scale, or because of performance pain?
Answer:

### Were there previous rewrite attempts that got abandoned?
Answer:

---

## Music System

### Is music still a major priority, or mostly legacy baggage now?
Answer:

### What parts of the old music system actually worked well?
Answer:

### What parts were cursed enough that you never want to touch them again?
Answer:

### What is still missing for the current music rewrite to reach parity with the old one?
Answer:

### Do you still want queue persistence and last-played persistence in the new architecture?
Answer:

### Should playlists remain DB-backed, or would you rather redesign them entirely?
Answer:

### Is `DISABLE_LAVALINK = true` just a local/dev safety toggle, or is that telling us something bigger?
Answer:

### Are there music features you intentionally dropped and do not want back?
Answer:

---

## Playlist / Persistence

### Are playlists still used enough to be worth finishing?
Answer:

### Were users actively relying on playlist import/export and fixed playlists?
Answer:

### Do you trust the old SQL schema conceptually, or do you consider it technical debt that should be replaced?
Answer:

### Is the SQL folder historical archive, still-used reference, or accidental repo luggage?
Answer:

### Should future persistence stay PostgreSQL, or are you open to something else?
Answer:

### Are there any data migration constraints from prod we have to preserve?
Answer:

---

## Commands / UX

### Which command groups do users actually use the most?
Answer:

### Which command groups do you use the most for testing?
Answer:

### Are there command groups you'd like to split further or simplify?
Answer:

### Do you prefer slash-command-first design going forward?
Answer:

### Do you want to keep `CommandsNext` around at all, or only as a compatibility prison for NSFW stuff?
Answer:

### Do you want more V2 component/UI usage across the bot, or only where it adds real value?
Answer:

---

## NSFW Module

### Is the NSFW module still relevant to prod?
Answer:

### Is it intentionally isolated forever, or do you eventually want it migrated too?
Answer:

### Would you ever want it removed from this repo entirely?
Answer:

### Are there policy / hosting / moderation reasons that make it annoying to maintain?
Answer:

---

## Developer / Maintainer Experience

### What parts of the repo are the most painful for you to work in?
Answer:

### What files do you dread opening?
Answer:

### What parts still feel "safe" and pleasant to change?
Answer:

### What kind of burnout did this project cause most: feature creep, support burden, Discord API churn, music bot hell, infra, community expectations, or maintenance alone?
Answer:

### When you come back to the project after a break, what usually blocks you first?
Answer:

### Is there missing documentation that would have helped Past Lala a lot?
Answer:

---

## Infra / Production

### How is prod actually hosted right now?
Answer:

### Is `hatsune-miku.service` representative of real deployment?
Answer:

### How many shards are you running in prod these days?
Answer:

### Is there any shard coordination outside this repo?
Answer:

### Are there external services the bot depends on that are flaky or gone now?
Answer:

### Which APIs in the repo are already dead, deprecated, rate-limited to hell, or untrusted?
Answer:

### Are there observability tools besides logs and Sentry that matter here?
Answer:

---

## Scale / Performance

### What are the biggest performance pain points you've seen in prod?
Answer:

### Did sharding solve the main scale issues, or just keep the bot alive?
Answer:

### Are there memory leaks, queue issues, or reconnect issues you already know about?
Answer:

### Are there commands or subsystems you avoid touching because they can blow up at scale?
Answer:

### Is DB load a real concern in prod, or not really anymore since parts moved in-memory?
Answer:

---

## DisCatSharp / Library Context

### Which DisCatSharp changes hurt this project the most over time?
Answer:

### Are there local DisCatSharp patches you depend on mentally even if this repo uses NuGet most of the time?
Answer:

### Is the `DebugSrc` setup something you actively use, or more of a safety hatch?
Answer:

### Are there specific library bugs or gaps that shaped the current code structure?
Answer:

---

## Data / Content

### Are the SQL dumps safe to keep around, or do you want them treated as sensitive historical baggage?
Answer:

### Are there any legal/privacy concerns around old stored data?
Answer:

### Are there assets, IDs, or config values in repo history that should eventually be cleaned up?
Answer:

### Are there old commands or features whose behavior users still remember and expect?
Answer:

---

## Testing / Reliability

### How do you currently validate changes: local manual testing, staging bot, direct prod canary, vibes?
Answer:

### Is there any staging environment or test bot for this project?
Answer:

### Are there areas where automated tests would actually help, versus just being maintenance cosplay?
Answer:

### Which regressions are the most likely when editing this repo?
Answer:

### Are there known flaky flows I should treat with extra suspicion?
Answer:

---

## Refactor Appetite

### How much change are you comfortable with at once?
Answer:

### Do you prefer incremental surgical fixes, or occasional bigger cleanup arcs?
Answer:

### Are you okay with deleting legacy code once parity is restored?
Answer:

### Do you want me to optimize for minimal risk first, or for long-term cleanup first?
Answer:

### If I propose architectural cleanup, should I be conservative unless asked otherwise?
Answer:

---

## Priority / Roadmap

### If we only fixed 3 things in the next month, what should they be?
Answer:

### If we only fixed 1 thing, what should it be?
Answer:

### What would make you feel the project is "alive again"?
Answer:

### What would reduce your dread the most?
Answer:

### What would make you actually enjoy touching this repo again?
Answer:

---

## Collaboration Preferences

### Do you want me to be more "pair programmer," more "maintainer archaeologist," or more "just do the work"?
Answer:

### Do you want me to keep making docs/maps as we learn more, or focus mostly on code?
Answer:

### Should I aggressively call out dead ends and risky ideas, even if that means being blunt?
Answer:

### Do you want me to preserve old behavior by default, or challenge old behavior if it seems not worth keeping?
Answer:

### When I notice something questionable, do you want design discussion first or patches first?
Answer:

---

## Freeform

### What prod actually uses
Answer:

### What I hate
Answer:

### What I still want
Answer:

### What is dead fr
Answer:

### What burned me out
Answer:

### What you should never touch lightly
Answer:
