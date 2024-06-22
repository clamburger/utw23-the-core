using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using UbertweakNfcReaderWeb.Messages;
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
        Task TeamUpdate(Team team);
        Task CardUpdate(Card card);
        Task PurchaseSuccessful(ShopItem item);
        Task ScannerUpdate();
        Task OptionUpdated(VoteOption option);
        Task OptionAdded(VoteOption option);
    }

    public class MessageHub : Hub<IMessageClient>
    {
        private readonly PlexusService _plexus;
        private readonly ScannerService _connectionManager;

        public MessageHub(PlexusService plexus, ScannerService connectionManager) {
            _plexus = plexus;
            _connectionManager = connectionManager;
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

            if (card.User != null)
            {
                card.User = db.Users.Find(card.User.Id);
            }
            
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

        public async Task RegisterSpecialRewardCard(string uid, int itemId, string label)
        {
            await using var db = new DatabaseContext();

            var item = db.ShopItems
                .Include(i => i.Owner)
                .FirstOrDefault(i => i.Id == itemId);

            if (item == null || item.Owner != null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Item is invalid or already has an owner.");
                }
                return;
            }
            
            item.Available = false;
            
            var card = new Card
            {
                Uid = uid,
                Number = label,
                Type = CardType.SpecialReward,
                Data = itemId.ToString(),
            };

            item.RewardCard = card;

            await RegisterCard(card);
        }

        public async Task RegisterPersonCard(string uid, int userId)
        {
            await using var db = new DatabaseContext();

            var user = db.Users.Find(userId);
            
            var card = new Card
            {
                Uid = uid,
                Type = CardType.Person,
                Data = userId.ToString(),
                User = user
            };

            await RegisterCard(card);
        }

        public async Task AddShopItem(string name, int type, int price, bool available)
        {
            await using var db = new DatabaseContext();

            var item = new ShopItem
            {
                Name = name,
                Type = (ShopItemType)type,
                Price = price,
                Available = available,
            };

            db.ShopItems.Add(item);
            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).SystemSuccess($"Shop item #{item.Id} created.");
            }
        }

        public async Task RedeemCard(string uid, int userId)
        {
            await using var db = new DatabaseContext();
            
            var card = db.Cards.FirstOrDefault(c => c.Uid == uid);
            var user = db.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.Id == userId);

            if (card == null || user?.Team == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid call.");
                }

                return;
            }
            
            if (card.Redeemed == true)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Card has already been redeemed.");
                }

                return;
            }

            string result;
            string category;

            switch (card.Type)
            {
                case CardType.Credits:
                {
                    user.Team.Balance += int.Parse(card.Data);
                
                    if (_plexus.PrimaryConnection != null)
                    {
                        await Clients.Client(_plexus.PrimaryConnection).SystemSuccess($"Card has been redeemed! {card.Data} credits have been added.");
                    }

                    result = $"+{card.Data} credits";
                    category = "credits";
                    break;
                }
                case CardType.SpecialReward:
                {
                    var item = db.ShopItems.Find(int.Parse(card.Data));
                    item.Owner = user.Team;
                    
                    if (_plexus.PrimaryConnection != null)
                    {
                        await Clients.Client(_plexus.PrimaryConnection).SystemSuccess("Card has been redeemed! Item unlocked in The Shop.");
                    }

                    result = $"Unlocked shop item \"{item.Name}\"";
                    category = "special";
                    break;
                }
                default:
                {
                    if (_plexus.PrimaryConnection != null)
                    {
                        await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid card type.");
                    }

                    return;
                }
            }

            var scan = new Scan
            {
                Card = card,
                User = user,
                Team = user.Team,
                DateTime = DateTime.Now,
                Result = result,
                Category = $"redeem/{category}"
            };

            db.Scans.Add(scan);

            card.Redeemed = true;
            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).TeamUpdate(user.Team);
                await Clients.Client(_plexus.PrimaryConnection).CardUpdate(card);
            }
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

            foreach (var card in db.Cards.Where(c => c.User != null))
            {
                db.Cards.Remove(card);
            }
            
            foreach (var vote in db.UserVotes)
            {
                db.UserVotes.Remove(vote);
            }
            
            await db.SaveChangesAsync();
            
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
                new Team { Name = "Cotton", Colour = "#FF00FF" },
                new Team { Name = "Bolger", Colour = "#0000FF" },
                new Team { Name = "Proudfoot", Colour = "#00FF00" },
                new Team { Name = "Underhill", Colour = "#274E13" },
                new Team { Name = "Boffin", Colour = "#FF0000" },
                new Team { Name = "Hornblower", Colour = "#434343" },
                new Team { Name = "Gamgee", Colour = "#FFFF00" },
                new Team { Name = "Whitfoot", Colour = "#FFFFFF" },
                new Team { Name = "Took", Colour = "#9900FF" }
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
                "Cook/Helper",
                "Team Member"
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
                    if (teamName == "Builders")
                    {
                        teamName = "Bulders";
                    }
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

        public async Task ResetCard(string uid, int userId)
        {
            await using var db = new DatabaseContext();

            var card = db.Cards.FirstOrDefault(c => c.Uid == uid);

            if (card == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid card.");
                }
                return;
            }

            string result;

            if (card.Redeemed == true)
            {
                card.Redeemed = false;
                result = "Card redemption reset";
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemSuccess($"Card redemption reset.");
                    await Clients.Client(_plexus.PrimaryConnection).CardUpdate(card);
                }
            }
            else
            {
                result = "Card not reset (not redemeed)";
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemSuccess($"Card not redeemed.");
                }
            }

            var scan = new Scan
            {
                Card = card,
                Category = "admin/reset",
                DateTime = DateTime.Now,
                Result = result,
                User = db.Users.Find(userId)
            };

            db.Scans.Add(scan);
            await db.SaveChangesAsync();
        }

        public async Task EmulateScanById(int id)
        {
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardRemoved();
            }
            
            await using var db = new DatabaseContext();

            var card = db.Cards
                .Include(c => c.User)
                .ThenInclude(u => u.Team)
                .FirstOrDefault(c => c.Id == id);

            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardInserted(card);
            }
        }

        public async Task EmulateScanByLabel(string label)
        {
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardRemoved();
            }

            await using var db = new DatabaseContext();

            var card = db.Cards
                .Include(c => c.User)
                .ThenInclude(u => u.Team)
                .FirstOrDefault(c => c.Number == label);

            if (card == null)
            {
                card = db.Cards
                    .Include(c => c.User)
                    .ThenInclude(u => u.Team)
                    .FirstOrDefault(c => c.User.Name == label);
            }

            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardInserted(card);
            }
        }

        public async Task EmulateScanByUid(string uid)
        {
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardRemoved();
            }
            
            await using var db = new DatabaseContext();
            
            AnyCard card = db.Cards
                .Include(c => c.User)
                .ThenInclude(u => u.Team)
                .FirstOrDefault(c => c.Uid == uid);

            if (card == null)
            {
                card = new AnyCard
                {
                    Uid = uid
                };
            }
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardInserted(card);
            }
        }

        public async Task EmulateCardRemoved()
        {
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).CardRemoved();
            }
        }

        public async Task ConfirmPurchase(int userId, int leaderId, int itemId)
        {
            await using var db = new DatabaseContext();

            var user = db.Users.Include(u => u.Team)
                .FirstOrDefault(u => u.Id == userId);
            var leader = db.Users.Find(leaderId);
            var item = db.ShopItems.Include(i => i.Owner)
                .FirstOrDefault(i => i.Id == itemId);

            if (item == null || user == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid user or card.");
                }

                return;
            }

            if (user.Team == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("User doesn't belong to a team.");
                }
                return;
            }

            // if (item.Available == false || item.Owner != null || user.Team.Balance < item.Price)
            // {
            //     if (_plexus.PrimaryConnection != null)
            //     {
            //         await Clients.Client(_plexus.PrimaryConnection).SystemError("Item is unavailable or team has an insufficient balance.");
            //     }
            //     return;
            // }

            if (item.Owner == null)
            {
                item.Owner = user.Team;
                user.Team.Balance -= item.Price;
            }
            
            item.Available = false;
            item.Redeemed = true;

            var purchase = new Purchase
            {
                DateTime = DateTime.Now,
                User = user,
                Leader = leader,
                ShopItem = item,
                Team = user.Team
            };

            db.Purchases.Add(purchase);

            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).PurchaseSuccessful(item);
                await Clients.Client(_plexus.PrimaryConnection).TeamUpdate(user.Team);
            }
        }

        public async Task LoggedIn(string uid, int userId)
        {
            await using var db = new DatabaseContext();
            
            var card = db.Cards.FirstOrDefault(c => c.Uid == uid);
            var user = db.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.Id == userId);

            var scan = new Scan
            {
                Card = card,
                User = user,
                Team = user.Team,
                Category = "auth",
                DateTime = DateTime.Now,
                Result = "Logged in"
            };

            db.Scans.Add(scan);
            await db.SaveChangesAsync();
        }

        public async Task UpdateScannerState(ScannerState state)
        {
            var message = new SetState
            {
                State = state
            };
            
            await _connectionManager.SendToAll(message);
        }

        public async void DisconnectScanners(List<string> connectionIds)
        {
            foreach (var connectionId in connectionIds)
            {
                _connectionManager.DisconnectScanner(connectionId);
            }

            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).ScannerUpdate();
            }
        }

        public async void UpdateOptionLabel(int id, string label)
        {
            await using var db = new DatabaseContext();
            
            var option = await db.VoteOptions.FindAsync(id);
            if (option == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid option.");
                }
                return;
            }

            option.Name = label;
            
            await db.SaveChangesAsync();
            
            await _connectionManager.SendToAll(new SetOptionTitle
            {
                Text = option.Name,
                OptionNumber = option.Number
            });

            if (!option.Enabled)
            {
                await _connectionManager.SendToAll(new SetOptionEnabled
                {
                    Enabled = option.Enabled ? 1 : 0,
                    OptionNumber = option.Number
                });
            }
        }

        public async Task UpdateOptionEnabled(int id, bool enabled)
        {
            await using var db = new DatabaseContext();
            
            var option = await db.VoteOptions.FindAsync(id);
            if (option == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid option.");
                }
                return;
            }

            option.Enabled = enabled;
            
            await db.SaveChangesAsync();
            
            await _connectionManager.SendToAll(new SetOptionEnabled
            {
                Enabled = option.Enabled ? 1 : 0,
                OptionNumber = option.Number
            });
        }
        
        public async Task UpdateOptionLimit(int id, int? limit)
        {
            await using var db = new DatabaseContext();
            
            var option = await db.VoteOptions.FindAsync(id);
            if (option == null)
            {
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).SystemError("Invalid option.");
                }
                return;
            }
            
            option.Limit = limit;
            
            await db.SaveChangesAsync();

            var voteCount = db.UserVotes
                .Count(v => v.Option.Id == option.Id);

            if (voteCount >= option.Limit)
            {
                option.Enabled = false;
            
                await db.SaveChangesAsync();
            
                await _connectionManager.SendToAll(new SetOptionEnabled
                {
                    Enabled = option.Enabled ? 1 : 0,
                    OptionNumber = option.Number
                });
                
                if (_plexus.PrimaryConnection != null)
                {
                    await Clients.Client(_plexus.PrimaryConnection).OptionUpdated(option);
                }
            }
        }

        public async Task DeleteOption(int id)
        {
            await using var db = new DatabaseContext();

            var votes = db.UserVotes.Where(v => v.Option.Id == id);
            foreach (var vote in votes)
            {
                db.UserVotes.Remove(vote);
            }

            await db.SaveChangesAsync();

            var deletingOption = await db.VoteOptions.FindAsync(id);
            if (deletingOption == null)
            {
                return;
            }

            var options = db.VoteOptions
                .Where(o => o.Number >= deletingOption.Number)
                .OrderBy(o => o.Number).ToList();
            
            var newNumber = 0;
            foreach (var option in options)
            {
                if (option.Id == id)
                {
                    db.VoteOptions.Remove(option);
                    continue;
                }

                newNumber += 1;
                option.Number = newNumber;
            }

            await db.SaveChangesAsync();

            await _connectionManager.UpdateAllSettings();
        }

        public async Task AddOption()
        {
            await using var db = new DatabaseContext();

            var number = db.VoteOptions.Count() + 1;

            var option = new VoteOption
            {
                Enabled = true,
                Name = "Option " + number,
                Number = number,
                Limit = null
            };

            await db.VoteOptions.AddAsync(option);
            await db.SaveChangesAsync();
            
            if (_plexus.PrimaryConnection != null)
            {
                await Clients.Client(_plexus.PrimaryConnection).OptionAdded(option);
            }
            
            await _connectionManager.SendToAll(new SetNumberOfOptions
            {
                OptionCount = option.Number
            });
            
            await _connectionManager.SendToAll(new SetOptionTitle
            {
                OptionNumber = option.Number,
                Text = option.Name
            });
        }

        public async Task ClearVotes()
        {
            await using var db = new DatabaseContext();

            foreach (var vote in db.UserVotes)
            {
                db.Remove(vote);
            }

            await db.SaveChangesAsync();
        }
    }
}
