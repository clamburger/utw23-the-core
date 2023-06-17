namespace UbertweakNfcReaderWeb.Models;

public class Person
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public bool? Leader { get; set; }
}