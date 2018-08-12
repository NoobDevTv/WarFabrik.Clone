using System;
using System.IO;
using System.Threading;
using Telegram.Bot;

namespace NoobDevBot.Telegram
{
    class Program
    {

        public static TelegramBot Bot;

        static void Main(string[] args)
        {
            AutoResetEvent are = new AutoResetEvent(false);

            Bot = new TelegramBot();

            DatabaseManager.Initialize();

            
        }

    }
}
