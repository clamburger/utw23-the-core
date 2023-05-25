using MediatR;
using PCSC.Monitoring;
using UbertweakNfcReaderWeb.Messaging;
using UbertweakNfcReaderWeb.Models;

namespace UbertweakNfcReaderWeb.Services
{
    public class PlexusService
    {
        private readonly NfcService _nfc;
        private readonly IMediator _mediator;

        public string? PrimaryConnection;

        public PlexusService(NfcService nfc, IMediator mediator)
        {
            _nfc = nfc;
            _mediator = mediator;
        }

        public void Watch()
        {
            _nfc.CardInserted += CardInserted;
            _nfc.CardRemoved += CardRemoved;
        }

        public void StopWatch()
        {
            _nfc.CardInserted -= CardInserted;
            _nfc.CardRemoved -= CardRemoved;
        }

        public void CardInserted(object? sender, CardInsertedEventArgs e)
        {
            using var db = new DatabaseContext();
            var card = db.Cards.FirstOrDefault(c => c.Uid == e.Uid);

            _mediator.Publish(new CardInserted { Card = card, Uid = e.Uid });
        }

        public void CardRemoved(object? sender, CardStatusEventArgs e)
        {
            _mediator.Publish(new CardRemoved());
        }
    }
}
