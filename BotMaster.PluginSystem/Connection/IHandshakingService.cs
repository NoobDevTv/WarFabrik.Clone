namespace BotMaster.PluginSystem.Connection;
public interface IHandshakingService
{
    Guid RegisterAsClient(PluginManifest manifest, Guid? fixedInstanceId);
    PluginConnection StartAsClient(PluginManifest manifest, Guid instanceId);
    void Start();
}