using MediatR;
using Microsoft.AspNetCore.SignalR;
using UbertweakNfcReaderWeb.Hubs;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb.Messaging
{
    public class CardRemoved : INotification {}

    public class CardRemovedHandler : INotificationHandler<CardRemoved>
    {
        private readonly PlexusService _plexus;
        private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

        public CardRemovedHandler(PlexusService plexus, IHubContext<MessageHub, IMessageClient> hubContext)
        {
            _plexus = plexus;
            _hubContext = hubContext;
        }

        public async Task Handle(CardRemoved request, CancellationToken cancellationToken)
        {
            if (_plexus.PrimaryConnection != null)
            {
                await _hubContext.Clients.Client(_plexus.PrimaryConnection).CardRemoved();
            }
        }
    }
}
