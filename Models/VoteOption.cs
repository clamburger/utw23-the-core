namespace UbertweakNfcReaderWeb.Models;

public class VoteOption
{
    public int Id { get; set; }
    public required int Number { get; set; }
    public required string Name { get; set; }
    public required int? Limit { get; set; }
    public required bool Enabled { get; set; }
}