using BotMaster.Betterplace.Controllers;
using BotMaster.Betterplace.MessageContract;
using BotMaster.Betterplace.Model;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;

using System.Reactive.Linq;

using ILogger = NLog.ILogger;

namespace BotMaster.Betterplace;

public sealed class BetterplaceService : Plugin
{
    private static void Main(string[] args)
    {
        HashSet<int> DonationIds = new();
        var Opinions
            = BetterplaceClient
            .GetOpinionsPage(File.ReadAllText("additionalfiles/BetterplaceEventId.txt"), TimeSpan.FromSeconds(20))
            .Retry()
            .SelectMany(p => p.Data)
            .Where(x => !DonationIds.Contains(x.Id))
            .Where(o => o.Created_at > DateTime.Now.Subtract(TimeSpan.FromMinutes(4)))
            .Do(x => DonationIds.Add(x.Id))
            .Publish()
            .RefCount();

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


        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSingleton(opinionsCache);
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }

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


    private async Task RunAspNetHost()
    {

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

        //var builder = WebApplication.CreateBuilder();
        //builder.Services.AddControllers();
        //builder.Services.AddHealthChecks();
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();
        //var app = builder.Build();

        //app.UseHttpsRedirection();

        //app.UseAuthorization();
        //app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        //    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
        //app.UseRouting();
        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllerRoute(
        //           name: "default",
        //           pattern: "{controller}/{action=Index}/{id?}");
        //});
        //app.UseSwagger();
        //app.UseSwaggerUI();
        ////app.MapControllers();
        //app.UseStaticFiles();
        //app.MapHealthChecks("/health");
        //await app.RunAsync();

        var builder = WebApplication.CreateBuilder(Array.Empty<String>());

        // Add services to the container.

        builder.Services.AddSingleton(opinionsCache);
        builder.Services.AddControllers().AddApplicationPart(typeof(BetterplaceController).Assembly);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        //app.UseSwagger();
        //app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
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
