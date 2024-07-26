using Software_hotel.Models;

namespace Software_hotel.DAO
{
    public interface IPrenontazioneDao
    {
        IEnumerable<Prenotazione> GetAll();
        IEnumerable<Prenotazione> GetPrenotazioneByCF(string CodiceFiscale);
        IEnumerable<Prenotazione> GetPrenotazioneByPensione(string TipoPensione);
        IEnumerable<ServiziAggiuntivi> GetListaServizi();
        decimal AggiungiServizio(int idPrenotazione, ServizioPerPrenotazione servizioPerPrenotazione);
        IEnumerable<ServizioPerPrenotazione> GetServiziPerPrenotazione(int idPrenotazione);

    }
}
