namespace UbertweakNfcReaderWeb.Models;

public class UserVote
{
    public int Id { get; set; }
    public required User User { get; set; }
    public required VoteOption Option { get; set; }
    public required DateTime DateTime { get; set; }
}