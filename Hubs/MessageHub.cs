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
        Task CardRegistered(Card card);
        Task SystemError(string message);
        Task SystemSuccess(string message);

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

        public async Task RegisterFirstAdminCard(string uid)
        {
            using var db = new DatabaseContext();

            var firstTimeSetup = db.Cards.Any(c => c.Type == CardType.Admin) == false;

            if (!firstTimeSetup)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("An admin card already exists - can't register another while in first time setup mode");
                    return;
                }
            }

            var existingCard = db.Cards.SingleOrDefault(c => c.Uid == uid);
            if (existingCard != null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Card already registered");
                    return;
                }
            }

            Random random = new Random();
            int num = random.Next();
            string hexString = num.ToString("X");

            var card = new Card {
                Uid = uid,
                Number = hexString,
                Type = CardType.Admin
            };

            db.Add<Card>(card);
            db.SaveChanges();

            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("Admin card registered.");
                await Clients.Client(_plexus.PrimaryConnection).CardRegistered(card);
            }
        }

        public async Task RegisterCard(string uid, CardType type, object arg1, object arg2)
        {
            using var db = new DatabaseContext();

            var existingCard = db.Cards.SingleOrDefault(c => c.Uid == uid);
            if (existingCard != null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Card already registered");
                }
                return;
            }
            
            Card card;

            Random random = new Random();
            int num = random.Next();
            string hexString = num.ToString("X");

            if (type == CardType.Admin) {
                card = new Card{
                    Uid = uid,
                    Type = type,
                    Number = hexString
                };
            } else if (type == CardType.Credits) {
                card = new Card{
                    Uid = uid,
                    Type = type,
                    Number = hexString,
                    Data = (string)arg1,
                };
            } else if (type == CardType.Team) {
                var name = (string)arg1;
                var colour = (string)arg2;

                var team = db.Teams.SingleOrDefault(c => c.Name == name);
                if (team == null) {
                    team = new Team{
                        Name = name,
                        Colour = colour,
                        Admin = false
                    };
                    db.Add<Team>(team);
                    db.SaveChanges();
                }

                card = new Card{
                    Uid =  uid,
                    Type = type,
                    Number = hexString,
                };
            } else {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Card not registered: Invalid card type");
                }
                return;
            }

            db.Add<Card>(card);
            db.SaveChanges();

            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("Card registered.");
                await Clients.Client(_plexus.PrimaryConnection).CardRegistered(card);
            }
        }
    }
}
