using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Spectre.Console;
using UbertweakNfcReaderWeb.Hubs;
using UbertweakNfcReaderWeb.Messaging;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddSingleton<NfcService>();
            builder.Services.AddSingleton<PlexusService>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.MapRazorPages();
            app.MapHub<MessageHub>("/api/hub");

            ShowWelcomeMessage();
            app.Run();
        }

        private static void ShowWelcomeMessage()
        {
            Panel panel = new(
                Align.Center(new Markup("[yellow1]Welcome to the �bertweak Central Plexus\n\nCore systems initialised.[/]"))
            )
            {
                Border = BoxBorder.Double,
                Padding = new Padding(1),
                Expand = true
            };
            panel.BorderColor(Color.Green1);
            AnsiConsole.Write(panel);
        }
    }

    public class Worker : IHostedService
    {
        public NfcService _nfcService;
        public PlexusService _plexusService;

        public Worker(NfcService nfcService, PlexusService plexusService)
        {
            _nfcService = nfcService;
            _plexusService = plexusService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _nfcService.Watch();
            _plexusService.Watch();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _nfcService.StopWatch();
            _plexusService.StopWatch();

            return Task.CompletedTask;
        }
    }
}