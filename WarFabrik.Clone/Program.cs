using System;
using System.Diagnostics;
using System.Threading;
using WarFabrik.Clone.Commands;

namespace WarFabrik.Clone
{
    class Program
    {
        private static Bot bot;

        static void Main(string[] args)
        {
            var stopper = new ManualResetEvent(false);
            
            bot = new Bot();

            bot.Connect();

            bot.FollowerServiceOnNewFollowersDetected(null, null);
            
            Console.ReadKey();
            bot.SendMessage("/me geht in den Standby");
            bot.Disconnect();
        }
    }
}
