namespace UbertweakNfcReaderWeb.Models;

public class Purchase
{
    public int Id { get; set; }
    public required ShopItem ShopItem { get; set; }
    public required DateTime DateTime { get; set; }
    public required User User { get; set; }
    public required User Leader { get; set; }
    public required Team Team { get; set; }
}