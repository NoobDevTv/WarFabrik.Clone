﻿using BotMaster.Betterplace;
using BotMaster.Betterplace.Model;
using NLog;
using NLog.Config;
using NLog.Fluent;
using NLog.Targets;
using NoobDevBot.Telegram;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WarFabrik.Clone;
using static WarFabrik.Clone.FollowerServiceNew;

namespace BotMaster.Runtime
{
    public sealed class Service
    {
        public ConcurrentQueue<Opinion> Opinions { get;  }

        private readonly Logger logger;
        private readonly CancellationTokenSource tokenSource;
        private readonly TelegramBot telegramBot;
        private readonly BetterplaceService betterplaceService;
        private readonly Bot twitchBot;
        private IDisposable betterplaceSub;

        public Service()
        {
            logger = LogManager.GetCurrentClassLogger();
            Opinions = new ConcurrentQueue<Opinion>();
            telegramBot = new TelegramBot();
            betterplaceService = new BetterplaceService();
            tokenSource = new CancellationTokenSource();
            twitchBot = new Bot();
        }

        public async Task Run()
        {          
            twitchBot.FollowerService.OnNewFollowersDetected += TwitchNewFollower;
            twitchBot.OnRaid += TwitchBotOnHosted;

            await twitchBot.Run(tokenSource.Token);

            logger.Info("Der Bot ist Online");
            telegramBot.SendMessageToGroup("NoobDev", "Der Bot ist Online");

            betterplaceSub = betterplaceService.Opinions.Subscribe(OnNewOpinion);
            
        }

        public void Stop()
        {
            logger.Info("Quit Bot master");

            twitchBot.FollowerService.OnNewFollowersDetected -= TwitchNewFollower;
            twitchBot.OnRaid -= TwitchBotOnHosted;

            telegramBot.Exit();
            twitchBot.Disconnect();
            tokenSource.Cancel();
            betterplaceSub.Dispose();
        }

        private void OnNewOpinion(Opinion obj)
        {
            var message = $"{(string.IsNullOrWhiteSpace(obj.Author?.Name) ? "Anonymer Noob" : obj.Author.Name)} hat {obj.Donated_amount_in_cents} Geld gespendet";
            telegramBot.SendMessageToGroup("NoobDev", message);
            twitchBot.SendMessage(message);
            Opinions.Enqueue(obj);
        }

        private void TwitchBotOnHosted(object sender, string message)
            => telegramBot.SendMessageToGroup("NoobDev", message);


        private void TwitchNewFollower(object sender, NewFollowerDetectedArgs e)
            => telegramBot.SendMessageToGroup("NoobDev", "Following people just followed: " + string.Join(", ", e.NewFollowers.Select(x => x.UserName)));
    }
}
