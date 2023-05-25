using Microsoft.AspNetCore.SignalR;
using Spectre.Console;
using UbertweakNfcReaderWeb.Models;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb.Hubs
{
    public interface IMessageClient
    {
        Task SystemMessage(string message);
        Task ScreenRegistered(bool firstTimeSetup);
        Task ScreenDeregistered();
        Task CardInserted(Card? card, string? uid);
        Task CardRemoved();
    }

    public class MessageHub : Hub<IMessageClient>
    {
        private readonly PlexusService _plexus;

        public MessageHub(PlexusService plexus) {
            _plexus = plexus;
        }

        public async Task RegisterPrimary()
        {
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).ScreenDeregistered();
            }

            _plexus.PrimaryConnection = Context.ConnectionId;

            using var db = new DatabaseContext();

            // Enter firstTimeSetup mode if there are no admin cards registered yet.
            var firstTimeSetup = db.Cards.Any(c => c.Type == CardType.Admin) == false;
            await Clients.Caller.ScreenRegistered(firstTimeSetup);
        }
    }
}
