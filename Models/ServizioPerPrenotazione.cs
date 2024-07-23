namespace Software_hotel.Models
{
    public class ServizioPerPrenotazione
    {
        public int idServizio {  get; set; }
        public int idSP {  get; set; }
        public int idPrenotazione { get; set; }
        public string DescrizioneServizio { get; set; }
        public decimal Prezzo { get; set; }
        public int Quantita { get; set; }

    }
}
