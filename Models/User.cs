namespace UbertweakNfcReaderWeb.Models;

public class User
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required bool Leader { get; set; }
    
    public Team? Team { get; set; }
}