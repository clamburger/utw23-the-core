namespace UbertweakNfcReaderWeb.Models;

public class ShopItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required ShopItemType Type { get; set; }
    public required int Price { get; set; }
    public Team? Owner { get; set; }
    public required bool Available { get; set; } = false;
}

public enum ShopItemType
{
    StandardLego,
    SpecialLego,
    SpecialReward
}