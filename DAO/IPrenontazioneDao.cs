using Software_hotel.Models;

namespace Software_hotel.DAO
{
    public interface IPrenontazioneDao
    {
        IEnumerable<Prenotazione> GetAll();
        IEnumerable<Prenotazione> GetPrenotazioneByCF(string CodiceFiscale);
        IEnumerable<Prenotazione> GetPrenotazioneByPensione(string TipoPensione);
        IEnumerable<ServizioPerPrenotazione> GetListaServizi();
        void AggiungiServizio(ServizioPerPrenotazione servizioPerPrenotazione);
    }
}
