namespace UbertweakNfcReaderWeb.Models
{
    public class Scan
    {
        public int Id { get; set; }
        public Card Card { get; set; }
        public DateTime DateTime { get; set; }
        public string Result { get; set; }
        public Team Team { get; set; }
    }
}
