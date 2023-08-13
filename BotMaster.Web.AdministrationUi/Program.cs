using BotMaster.Commandos;
using BotMaster.PluginSystem;
using BotMaster.RightsManagement;
using BotMaster.Web.AdministrationUi.Data;

using NLog;

using NLog.Web;
using System.Reactive.Disposables;
using BotMaster.PluginHost;
using System.Reactive.Linq;
using BotMaster.Web.AdministrationUi;
using BotMaster.PluginSystem.Connection;
using BotMaster.PluginSystem.Connection.TCP;
using System.IO.Pipes;

internal class Program
{
    internal static SystemMessageHandler SystemMessageHandler = new SystemMessageHandler();
    private static void Main(string[] args)
    {

        using var loggerShutdown = Disposable.Create(() => LogManager.Shutdown());

        var logger = LogManager
            .Setup()
            .LoadConfigurationFromAppSettings()
            .GetCurrentClassLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseNLog();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<EntityService<CommandosDbContext, PersistentCommand>>();
        builder.Services.AddSingleton<EntityService<RightsDbContext, User>>();
        builder.Services.AddSingleton<EntityService<RightsDbContext, PlattformUser>>();
        builder.Services.AddSingleton<EntityService<RightsDbContext, Right>>();
        builder.Services.AddSingleton<EntityService<RightsDbContext, Group>>();
        builder.Services.AddSingleton<EntityService<UserConnectionContext, UserConnection>>();
        builder.Services.AddSingleton(SystemMessageHandler);

        builder.Services.AddScoped<Radzen.DialogService>();
        builder.Services.AddScoped<Radzen.NotificationService>();
        builder.Services.AddScoped<Radzen.TooltipService>();
        builder.Services.AddScoped<Radzen.ContextMenuService>();


        var app = builder.Build();

        
        var debug = builder.Configuration.GetSection("Debug");
        var wait = debug.GetSection("Wait");
        if (!string.IsNullOrWhiteSpace(wait.Value) && int.TryParse(wait.Value, out var value))
        {
            logger.Debug($"Waiting for {value}");
            Thread.Sleep(value);
        }
        else
        {
            logger.Debug("Not waiting");
        }
        _ = StartPluginLogic(args, logger);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();


        async Task StartPluginLogic(string[] args, Logger logger)
        {
            var plugins = new List<Plugin>();
            logger.Debug("Gotten the following args: " + string.Join(" | ", args));

            try
            {
                FileInfo fi = default;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        i++;
                         fi =new FileInfo(args[i]);
                    }
                }

                var processCreator = new ConnectionProvider();
                var tcp = new TCPHandshakingService(processCreator);

                _ = await PluginHoster.Load(logger, processCreator, fi, false)
                    .IgnoreElements()
                    .Do(p => { }, ex =>
                    {
                        logger.Fatal(ex);
                        Environment.Exit(111);
                    })
                    ;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }
    }
}
