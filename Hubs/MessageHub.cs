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
        Task CardInserted(AnyCard card);
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

        private bool CheckIfCardExists(string uid)
        {
            using var db = new DatabaseContext();
            
            var existingCard = db.Cards.SingleOrDefault(c => c.Uid == uid);
            if (existingCard == null) return false;
            
            if (_plexus.PrimaryConnection != null)
            {
                Clients.Client(_plexus.PrimaryConnection).SystemError("Card already registered.");
            }
            
            return true;
        }

        private async Task RegisterCard(Card card)
        {
            await using var db = new DatabaseContext();

            if (CheckIfCardExists(card.Uid)) return;
            
            db.Add(card);
            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("Card registered.");
                await Clients.Client(_plexus.PrimaryConnection).CardRegistered(card);
            }
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
                }
                return;
            }

            await RegisterAdminCard(uid);
        }

        public async Task RegisterAdminCard(string uid, string? label = null)
        {
            var card = new Card {
                Uid = uid,
                Type = CardType.Admin,
                Number = label
            };
            
            await RegisterCard(card);
        }

        public async Task RegisterCreditsCard(string uid, int credits, string? label)
        {
            var card = new Card
            {
                Uid = uid,
                Type = CardType.Credits,
                Number = label,
                Data = credits.ToString()
            };

            await RegisterCard(card);
        }

        public async Task RegisterPersonCard(string uid, int userId)
        {
            var card = new Card
            {
                Uid = uid,
                Type = CardType.Person,
                Data = userId.ToString()
            };

            await RegisterCard(card);
        }

        public async Task UnregisterAllCards()
        {
            await using var db = new DatabaseContext();
            
            foreach (var card in db.Cards)
            {
                db.Cards.Remove(card);
            }

            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("All cards unregistered.");
            }
        }

        public async Task RemoveAllUsers()
        {
            await using var db = new DatabaseContext();
            
            foreach (var user in db.Users)
            {
                db.Users.Remove(user);
            }

            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("All users have been removed.");
            }
        }

        public async Task RecreateTeams()
        {
            await using var db = new DatabaseContext();
            
            if (db.Users.Any())
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Unable to recreate teams - users must be removed first.");
                }
            }
            
            // Remove existing teams
            foreach (var team in db.Teams)
            {
                db.Teams.Remove(team);
            }

            await db.SaveChangesAsync();
            
            // Recreate the teams
            var teams = new[]
            {
                new Team { Name = "Makers", Colour = "#002966" },
                new Team { Name = "Fixers", Colour = "#B8AC00" },
                new Team { Name = "Chargers", Colour = "#0B3B0B" },
                new Team { Name = "Helpers", Colour = "#E1E1E1" },
                new Team { Name = "Finders", Colour = "#6A2F96" },
                new Team { Name = "Transporters", Colour = "#940300" },
                new Team { Name = "Bulders", Colour = "#141414" }
            };

            foreach (var team in teams)
            {
                db.Teams.Add(team);
            }

            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("All teams have been removed and recreated.");
            }
        }

        public async Task ImportUsers(string data)
        {
            await using var db = new DatabaseContext();
            
            var lines = data
                .Split("\n")
                .Select(line => line.TrimEnd().Split("\t"))
                .Where(line => line.Length >= 3 && line[0] != "Type Name");

            var leaderRoles = new[] {
                "Volunteer",
                "Cook/Helper"
            };

            var users = lines.ToList();
            foreach (var line in users)
            {
                var user = new User
                {
                    Name = $"{line[1]} {line[2]}",
                    Leader = leaderRoles.Contains(line[0]),
                };

                try
                {
                    var teamName = line[5];
                    var team = db.Teams.FirstOrDefault(team => team.Name == teamName);
                    if (team != null) user.Team = team;
                }
                catch (IndexOutOfRangeException) {}

                db.Users.Add(user);
            }

            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess($"{users.Count} users imported.");
            }
        }
    }
}
