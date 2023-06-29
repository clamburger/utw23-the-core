namespace UbertweakNfcReaderWeb.Models
{
    public class Team
    {
        public Team()
        {
            Balance = 0;
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public string? Pin { get; set; }
        public int Balance { get; set; }
        public ICollection<ShopItem> ShopItems { get; } = new List<ShopItem>();
    }
}
