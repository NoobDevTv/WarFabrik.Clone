﻿using System.IO.Pipes;

namespace BotMaster.PluginSystem;

public interface IPluginInstanceCreator
{
    PluginInstance CreateServer(
            PluginManifest manifest,
            DirectoryInfo runnersPath);

    PluginInstance CreateClient(
            PluginManifest manifest);
}