using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbertweakNfcReaderWeb.Models;

namespace UbertweakNfcReaderWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly DatabaseContext _db = new();
    
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
            .ToList();

        return items;
    }
}

public class AuthenticationScheme
{
    
}