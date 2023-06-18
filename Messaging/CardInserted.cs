using MediatR;
using Microsoft.AspNetCore.SignalR;
using UbertweakNfcReaderWeb.Hubs;
using UbertweakNfcReaderWeb.Models;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb.Messaging
{
    public class CardInserted : INotification
    {
        public required AnyCard Card { get; init; }
    }

    public class CardInsertedHandler : INotificationHandler<CardInserted>
    {
        private readonly PlexusService _plexus;
        private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

        public CardInsertedHandler(PlexusService plexus, IHubContext<MessageHub, IMessageClient> hubContext)
        {
            _plexus = plexus;
            _hubContext = hubContext;
        }

        public async Task Handle(CardInserted request, CancellationToken cancellationToken)
        {
            if (_plexus.PrimaryConnection != null)
            {
                await _hubContext.Clients.Client(_plexus.PrimaryConnection).CardInserted(request.Card);
            }
        }
    }
}
