using BotMaster.Commandos;
using BotMaster.RightsManagement;
using BotMaster.Web.AdministrationUi.Data;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<EntityService<CommandosDbContext, PersistentCommand>>();
builder.Services.AddSingleton<EntityService<RightsDbContext, User>>();
builder.Services.AddSingleton<EntityService<RightsDbContext, PlattformUser>>();
builder.Services.AddSingleton<EntityService<RightsDbContext, Right>>();
builder.Services.AddSingleton<EntityService<RightsDbContext, BotMaster.RightsManagement.Group>>();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

var app = builder.Build();

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
