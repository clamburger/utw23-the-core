namespace UbertweakNfcReaderWeb.Models;

public class AnyCard
{
    public required string Uid { get; set; }
}

public class Card : AnyCard
{
    public int Id { get; set; }
    public string? Number { get; set; }
    public required CardType Type { get; set; }
    public bool? Redeemed { get; set; }
    public string? Pin { get; set; }
    public string? Data { get; set; }
    public bool? Enabled { get; set; } = true;
}

public enum CardType
{
    Admin,
    Person,
    Team,
    Credits,
    Reward,
    Special
}