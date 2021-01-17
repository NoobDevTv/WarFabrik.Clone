using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            using var resetEvent = new ManualResetEvent(false);


            resetEvent.WaitOne();
        }
    }
}
