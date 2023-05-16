using Microsoft.AspNetCore.SignalR;
using Spectre.Console;

namespace UbertweakNfcReaderWeb.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            AnsiConsole.MarkupLineInterpolated($"[blue]{user}:[/] [white]{message}[/]");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
