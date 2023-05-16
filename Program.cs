using Spectre.Console;
using UbertweakNfcReaderWeb.Hubs;
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
            builder.Services.AddSingleton<IHostedService, NfcService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<MessageHub>("/messageHub");

            ShowWelcomeMessage();
            app.Run();
        }

        private static void ShowWelcomeMessage()
        {
            Panel panel = new(
                Align.Center(new Markup("[yellow1]Welcome to the Übertweak Central Plexus\n\nCore systems initialised.[/]"))
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
}