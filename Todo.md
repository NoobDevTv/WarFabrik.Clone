TODOS:
	- ASP Net Core Web Core and Extensions
		- Betterplace donation alert

	- Save IDs of follows / donations, so we know which are new 

	- Appconfig per Plugin

	- (Fix self message on twitch?)
	
	- Support multiple runners

	- Youtube Bot API implementation
		- Set Title on Youtube and Twitch?
			- https://dev.twitch.tv/docs/api/reference#modify-channel-information
		- Message Relay YT <=> Twitch <=> (Telegram, maybe 2nd Bot?)
	
	- Add Twitter API (For Scheduled Events)

	- Discord BOT Api?

	- Database Problem? How To Why and When
		- Global Commands
		- Twitch/YT Command Message Creation
		- Save current followers, so we know the diff

	- Betterplace Optimizing (Webhooks)

	- Stresstest

	- TeamSpeak Bot

	- Cleanup again

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