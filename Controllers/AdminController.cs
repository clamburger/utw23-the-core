using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbertweakNfcReaderWeb.Models;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly DatabaseContext _db = new();
    
    private readonly ScannerService _connectionManager;

    public AdminController(ScannerService connectionManager)
    {
        _connectionManager = connectionManager;
    }

    [HttpGet("users")]
    public ActionResult<List<UserDto>> Users()
    {
        var cards = _db.Cards.Where(c => c.Type == CardType.Person);

        var users = _db.Users
            .Include(u => u.Team)
            .ToList()
            .Select(u =>
            {
                return new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Leader = u.Leader,
                    Team = u.Team,
                    Cards = cards.Where(c => c.Data == u.Id.ToString()).ToArray()
                };
            })
            .ToList();

        return users;
    }

    public class UserDto : User
    {
        public Card[] Cards { get; set; } = Array.Empty<Card>();
    }

    [HttpGet("shop-items")]
    public ActionResult<List<ShopItem>> ShopItems()
    {
        var items = _db.ShopItems
            .Include(i => i.Owner)
            .Include(i => i.RewardCard)
            .ToList();

        return items;
    }

    [HttpGet("teams")]
    public ActionResult<List<Team>> Teams()
    {
        var teams = _db.Teams
            .Include(t => t.ShopItems)
            .ToList();

        return teams;
    }
    
    [HttpGet("scanners")]
    public ActionResult<List<Scanner>> Scanners()
    {
        return _connectionManager.GetScanners();
    }

    public class VoteResultsDto
    {
        public List<VoteOption> Options { get; set; }
        public List<UserVote> Votes { get; set; }
    }
    
    [HttpGet("vote-results")]
    public ActionResult<VoteResultsDto> VoteResults()
    {
        var options = _db.VoteOptions
            .OrderBy(o => o.Number)
            .ToList();
        
        var votes = _db.UserVotes
            .Include(v => v.User)
            .Include(v => v.Option)
            .ToList();

        return new VoteResultsDto
        {
            Options = options,
            Votes = votes
        };
    }
    
    [HttpGet("poll-options")]
    public ActionResult<List<VoteOption>> PollOptions()
    {
        var options = _db.VoteOptions
            .OrderBy(o => o.Number)
            .ToList();

        return options;
    }
}