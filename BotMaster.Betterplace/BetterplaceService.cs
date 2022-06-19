using BotMaster.Betterplace.MessageContract;
using BotMaster.Betterplace.Model;
using BotMaster.Core;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NLog;
using NLog.Web;

using System.Reactive.Linq;

using ILogger = NLog.ILogger;

namespace BotMaster.Betterplace;

public sealed class BetterplaceService : Plugin
{
    public IObservable<Opinion> Opinions { get; private set; }

    private HashSet<int> DonationIds = new();

    public BetterplaceService()
    {
    }

    public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
    {
        if (Opinions is null)
            Opinions
                = BetterplaceClient
                .GetOpinionsPage(File.ReadAllText("additionalfiles/BetterplaceEventId.txt"), TimeSpan.FromSeconds(20))
                .Retry()
                .SelectMany(p => p.Data)
                .Where(x => !DonationIds.Contains(x.Id))
                .Where(o => o.Created_at > DateTime.Now.Subtract(TimeSpan.FromMinutes(4)))
                .Do(x => DonationIds.Add(x.Id))
                .Publish()
                .RefCount();

        RunAspNetHost();


        return MessageConvert
                    .ToPackage(
                  Create(logger,
                      MessageConvert.ToMessage(receivedPackages)
                    ));
    }


    private void RunAspNetHost()
    {
        var builder = WebApplication.CreateBuilder();

        var queue = new Queue<Alert>();
        var currentSubscription = Opinions
            .Select(x => new Alert(x.Id, x.Author?.Name, x.Message, x.Donated_amount_in_cents, x.Created_at))
            .Do(alert =>
            {
                queue.Enqueue(alert);
                if (queue.Count > 10)
                    queue.Dequeue();

            })
            .Subscribe();
        var opinionsCache = new OpinionsCache(currentSubscription, queue);

        builder.Services.AddControllers();
        builder.Services.AddSingleton(opinionsCache);
        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }

    public IObservable<Message> Create(ILogger logger, IObservable<Message> receivedPackages)
    {
        return BetterplaceContract.ToMessages(Opinions.Select(x => new BetterplaceMessage(x.ToDonation())));
    }
}

public record OpinionsCache(IDisposable CurrentSubscription, Queue<Alert> Alerts) : IDisposable
{
    public void Dispose()
    {
        CurrentSubscription?.Dispose();
    }
}
