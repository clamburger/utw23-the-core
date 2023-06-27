namespace UbertweakNfcReaderWeb.Models
{
    public class Scan
    {
        public int Id { get; set; }
        public required Card Card { get; set; }
        public required DateTime DateTime { get; set; }
        public required string Result { get; set; }
        public Team? Team { get; set; }
        public required User User { get; set; }
        public string? Category { get; set; }
    }
}
