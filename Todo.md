TODOS:
	- Rework Twitch to use more reactive instead of events
		- Cleanup FollowerService
		- Send Twitch messages / Command to the ouside
	
	- Youtube Bot API implementation
		- Set Title on Youtube and Twitch?
			- https://dev.twitch.tv/docs/api/reference#modify-channel-information
		- Message Relay YT <=> Twitch <=> (Telegram, maybe 2nd Bot?)
	
	- Add Twitter API (For Scheduled Events)

	- Change Message UID To GUID UID from Contracts

	- Database Problem? How To Why and When
		- Global Commands
		- Twitch/YT Command Message Creation
		- Save current followers, so we know the diff

	- Betterplace Optimizing

	- Stresstest


Done:
	- Dispatch Command (always) to itself
	- Botmaster Twitch & Telegram to library