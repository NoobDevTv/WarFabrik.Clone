TODOS:
	- Plugins Store Install / As docker container?
		- ✓ Standalone Database Plugin 
		- ✓ TCP Plugin Creator 
		- ✓ Manifests Location, Information, etc. pp.
		- ✓ reinstantiate connection when plugin docker reboot
		- ✓ Telegram crashes after 3 messages :(
		- Twitch duplicates messages when multiple plugins are running (Twitch Messages ping pong their messages) //Multiple Plugins of the same type is currently not supported in a single botmaster instance (May change in the future)
		- ✓ twitch not working after other plugins are reinstanciated
		- ✓ botmaster should reboot plugin when error inside plugin 
			- docker inspect c2c769c4b9ef --format='{{.State.ExitCode}}'  == 111 //We need an use case first, before we will implement this, because docker can do this for us via RestartPolicy
			✓ We need to correctly pass exception into dotnetRunner and throw them there, so we have the right exit code
		- ✓ existing container detection
		- ✓ betterplace bad gateway in own container

	- Fix YT again (Especially token generation)

	- Bots / Restream Bot is not allowed to execute commands

	- ASP Net Core Web Core and Extensions
		- ✓(Maybe) Betterplace donation alert
		- Web Hook / Call for Twitch / YT Follows
	- Betterplace Optimizing (Webhooks)
	
	- Improve Logging

	- Set Title on Youtube and Twitch?
		- https://dev.twitch.tv/docs/api/reference#modify-channel-information
	
	- Add Twitter API (For Scheduled Events)

	- Discord BOT Api?

	- Save IDs of follows / donations, so we know which are new 

	- Stresstest

	- TeamSpeak / Mumble Bot

	- Cleanup again

	- WebUi for Config, Logging, etc. pp. (Maybe as a sunday project)
		- DB:
			- Usermanagement of DB (Rights, Links, IDs, Delete, Create)
			- Commandmanagement (Delete, Create, Update)
		- Integration into Botmaster
			- Systemmessagecontract
				- Pluginmanagement Messages
		- Pluginmanagement:
			- PluginInstance List
				- Start & Stop
				- State
				- Create (UI for plugin.manifest.json)
				- Update (Docker Images or something)
				- Config of this plugin
			- Set Title on Youtube and Twitch / Twitter Notification
		- Testing:
			- LogView in General (Per Plugin aswell)
			- Demo Messages for Tests
			- Other Logs (Not just file)
		- Other:
			- Think about name
			- Authentication
		
	- Auto Update Docker Plugin Container and Botmaster (Self Update / Plugin Update Plugin etc.)

	- !dice Command

	Blazor				=> Kopf
	Blazor WebAssembly	=> Zahl


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