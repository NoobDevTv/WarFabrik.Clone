﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.Core.Plugins
{
    public class PluginServiceInstance : PluginInstance
    {
        private readonly Process process;

        public PluginServiceInstance(
            PluginManifest manifest, Process process, Func<IObservable<Package>, IObservable<Package>> createServer)
            : base(manifest, createServer)
        {
            this.process = process;
        }

        internal void Kill()
        {
            process.Kill(true);
            process.WaitForExit(1000);
        }

        internal bool TryStop()
        {
            return process.WaitForExit(10000);
        }

        public override void Start()
        {
            base.Start();
            //process.Start();
        }
    }
}