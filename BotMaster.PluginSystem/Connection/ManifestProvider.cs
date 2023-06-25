using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.PluginSystem.Connection;
public class ManifestProvider
{
    private readonly Subject<(PluginManifest manifest, Guid instanceId)> internalSubject;
    private readonly IObservable<(PluginManifest manifest, Guid instanceId)> internalStream;
    private readonly Dictionary<Guid, PluginManifest> currentDictionary;

    public ManifestProvider()
    {
        internalSubject = new();

        currentDictionary = new Dictionary<Guid, PluginManifest>();

        internalStream
            = Observable
            .Defer(() =>
                currentDictionary
                .Select(kvp => (kvp.Value, kvp.Key))
                .ToObservable()
                .Concat(internalSubject)
            );
    }

    public IObservable<(PluginManifest manifest, Guid instanceId)> GetStream()
    {
        return internalStream;
    }

    public IDisposable RegisterSubProvider(IObservable<(PluginManifest manifest, Guid instanceId)> provider)
    {
        return provider
            .Do(newData => currentDictionary[newData.instanceId] = newData.manifest)
            .Subscribe(internalSubject);
    }

    public void Dispose()
    {
        internalSubject.Dispose();
    }
}
