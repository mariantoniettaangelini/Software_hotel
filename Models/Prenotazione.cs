namespace Software_hotel.Models
{
    public class Prenotazione
    {
        public int idPrenotazione { get; set; }
        public Cliente cliente { get; set; }
        public Camera camera { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public DateTime InizioSoggiorno { get; set; }
        public DateTime FineSoggiorno { get; set; }
        public decimal Caparra { get; set; }
        public decimal Tariffa { get; set; }
        public string TipoPensione { get; set; }
        public string DescrizioneStanza { get; set; }
        public decimal TotaleServiziAggiuntivi { get; set; }
        public List<ServizioPerPrenotazione> serviziPerPrenotazione { get; set;  }
    }
}
