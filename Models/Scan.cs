namespace UbertweakNfcReaderWeb.Models
{
    public class Scan
    {
        public required int Id { get; set; }
        public required Card Card { get; set; }
        public required DateTime DateTime { get; set; }
        public required string Result { get; set; }
        public required Team Team { get; set; }
    }
}
