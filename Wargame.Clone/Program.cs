using System;
using System.Threading;

namespace Wargame.Clone
{
    class Program
    {

        static void Main(string[] args)
        {
            var stopper = new ManualResetEvent(false);
            var bot = new Bot();
            stopper.WaitOne();
        }
    }
}
