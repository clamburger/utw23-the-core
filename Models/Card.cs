namespace UbertweakNfcReaderWeb.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Number { get; set; }
        public CardType Type { get; set; }
        public bool? Redeemed { get; set; }
        public string? Pin { get; set; }
        public int Credits { get; set; }
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
