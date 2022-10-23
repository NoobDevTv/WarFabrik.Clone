TODOS:
	- ASP Net Core Web Core and Extensions
		- ✓(Maybe) Betterplace donation alert
		- Web Hook / Call for Twitch / YT Follows
	
	- Improve Logging

	- Set Title on Youtube and Twitch?
		- https://dev.twitch.tv/docs/api/reference#modify-channel-information
	
	- Add Twitter API (For Scheduled Events)

	- Discord BOT Api?

	- Save IDs of follows / donations, so we know which are new 

	- Betterplace Optimizing (Webhooks)

	- Stresstest

	- TeamSpeak Bot

	- Cleanup again

	- Plugins Store Install / As docker container?
		- Standalone Database Plugin 
		- TCP Plugin Creator
		- Manifests Location, Information, etc. pp.

	- WebUi for Config, Logging, Start & Stop, etc. pp.

	To Fix:
		- Telegram goes offline
		- Twitch send 4 messages on follow after longer runtime
		- No Twitch reconnect after restart

How To Migrate:
- Add reference of target to botmaster
- cd into Botmaster
- execute: dotnet ef migrations add MIGRATIONNAME --project ../TARGETPROJECT
- Remove reference of target to botmaster


Done:
	- Dispatch Command (always) to itself
	- Botmaster Twitch & Telegram to library

	- Rework Twitch to use more reactive instead of events
		- ✓ Cleanup FollowerService
		- ✓ Send Twitch messages / Command to the ouside
		- ✓ Rework BotCommandManager (Look @ Rule Of Silvester)
		

	- Rightsmanagement (Platform User mandatory, User currently handy)

	- ✓ Fix follower notification for twitch
	- ✓ Change Message UID To GUID UID from Contracts
	- ✓ Write own migration stuff so plugins can have own tables and so on

	- ✓ Fix needing to subscribe receive to be able to send packages
	
	- ✓ Youtube Bot API implementation
		- ✓  Message Relay YT <=> Twitch <(Not for now)> (Telegram, maybe 2nd Bot?)

	- ✓ (Fix self message on twitch?)

	- ✓ Exception inside Plugin shouldn't crash the bot
		- ✓ Does the external process get restarted after exception? No

	- ✓ Support multiple runners

	- ✓ Telegram => Subscribe to Notifications

	- ✓ Configuration Files
		- ✓ Move Nlog to Config File
		- ✓ Appconfig per Plugin
		- ✓ Appconfig for core service

	- Database Problem? How To Why and When
		- ✓ Global Commands
		- ✓ Twitch/ ✓ YT Command Message Creation