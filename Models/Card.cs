namespace UbertweakNfcReaderWeb.Models
{
    public class Card
    {
        public required int Id { get; set; }
        public required string Uid { get; set; }
        public required string Number { get; set; }
        public required CardType Type { get; set; }
        public bool? Redeemed { get; set; }
        public string? Pin { get; set; }
        public required int Credits { get; set; }
    }

    public enum CardType
    {
        Admin,
        Team,
        Credits,
        Reward,
        Special
    }
}
