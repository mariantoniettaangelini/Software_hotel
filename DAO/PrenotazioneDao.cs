using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Software_hotel.Models;
using Software_hotel.Services;

namespace Software_hotel.DAO
{
    public class PrenotazioneDao : SqlServiceSvcBase, IPrenontazioneDao
    {
        private const string SELECT_ALL_PRENOTAZIONI = @" 
        SELECT p.idPrenotazione, c.CodiceFiscale, c.Cognome, c.Nome, c.Citta, c.Provincia, c.Email, c.Telefono, c.Cellulare,
        ca.Descrizione AS DescrizioneStanza, ca.Tipologia,
        p.DataPrenotazione, p.DataInizioSoggiorno, p.DataFineSoggiorno, p.Caparra, p.Tariffa, p.TipoPensione,
        sp.idSP, sp.Prezzo, sp.Quantita, sa.DescrizioneServizio
        FROM Prenotazioni p
        INNER JOIN Clienti c ON p.idCliente = c.idCliente
        INNER JOIN Camera ca ON p.idCamera = ca.idCamera
        LEFT JOIN ServiziPerPrenotazione sp ON p.idPrenotazione = sp.idPrenotazione
        LEFT JOIN ServiziAggiuntivi sa ON sp.idServizio = sa.idServizio";

        private const string SELECT_BY_CF = @"
        SELECT p.idPrenotazione, c.CodiceFiscale, c.Cognome, c.Nome, c.Citta, c.Provincia, c.Email, c.Telefono, c.Cellulare,
        ca.Descrizione AS DescrizioneStanza, ca.Tipologia,
        p.DataPrenotazione, p.DataInizioSoggiorno, p.DataFineSoggiorno, p.Caparra, p.Tariffa, p.TipoPensione,
        sp.idSP, sp.Prezzo, sp.Quantita, sa.DescrizioneServizio
        FROM Prenotazioni p
        INNER JOIN Clienti c ON p.idCliente = c.idCliente
        INNER JOIN Camera ca ON p.idCamera = ca.idCamera
        LEFT JOIN ServiziPerPrenotazione sp ON p.idPrenotazione = sp.idPrenotazione
        LEFT JOIN ServiziAggiuntivi sa ON sp.idServizio = sa.idServizio
        WHERE c.CodiceFiscale LIKE @CodiceFiscale + '%'";

        private const string SELECT_ALL_SERVIZI = @"
        SELECT sp.idSP, sp.idServizio, sp.idPrenotazione, sp.Prezzo, sp.Quantita, sa.DescrizioneServizio
        FROM ServiziPerPrenotazione sp
        INNER JOIN ServiziAggiuntivi sa ON sp.idServizio = sa.idServizio";
        public PrenotazioneDao(IConfiguration config) : base(config)
        {
        }

        public void AggiungiServizio(ServizioPerPrenotazione servizioPerPrenotazione)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Prenotazione> GetAll()
        {
            var prenotazioni = new List<Prenotazione>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_ALL_PRENOTAZIONI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var prenotazione = new Prenotazione
                    {
                        idPrenotazione = reader.GetInt32(0),
                        cliente = new Cliente
                        {
                            CodiceFiscale = reader.GetString(1),
                            Cognome = reader.GetString(2),
                            Nome = reader.GetString(3),
                            Citta = reader.GetString(4),
                            Provincia = reader.GetString(5),
                            Email = reader.GetString(6),
                            Telefono = reader.GetString(7),
                            Cellulare = reader.GetString(8),
                        },
                        camera = new Camera
                        {
                            Descrizione = reader.GetString(9),
                            Tipologia = reader.GetString(10),
                        },
                        DataPrenotazione = reader.GetDateTime(11),
                        InizioSoggiorno = reader.GetDateTime(12),
                        FineSoggiorno = reader.GetDateTime(13),
                        Caparra = reader.GetDecimal(14),
                        Tariffa = reader.GetDecimal(15),
                        TipoPensione = reader.IsDBNull(16) ? null : reader.GetString(16),
                        serviziPerPrenotazione = new List<ServizioPerPrenotazione>()
                    };
                    if (!reader.IsDBNull(17)) 
                    {
                        prenotazione.serviziPerPrenotazione.Add(new ServizioPerPrenotazione
                        {
                            idSP = reader.GetInt32(17),
                            Prezzo = reader.GetDecimal(18),
                            Quantita = reader.GetInt32(19),
                            DescrizioneServizio = reader.GetString(20)
                        });
                    }
                    prenotazioni.Add(prenotazione);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni", ex);
            }

            return prenotazioni;
        }

        public IEnumerable<ServizioPerPrenotazione> GetListaServizi()
        {
            var servizi = new List<ServizioPerPrenotazione>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_ALL_SERVIZI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var servizio = new ServizioPerPrenotazione
                    {
                        idSP = reader.GetInt32(0),
                        idServizio = reader.GetInt32(1),
                        idPrenotazione = reader.GetInt32(2),
                        Prezzo = reader.GetDecimal(3),
                        Quantita = reader.GetInt32(4),
                        DescrizioneServizio = reader.GetString(5)
                    };
                    servizi.Add(servizio);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero dei servizi aggiuntivi", ex);
            }
            return servizi;
        }

        public IEnumerable<Prenotazione> GetPrenotazioneByCF(string CodiceFiscale)
        {
            var prenotazioni = new List<Prenotazione>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_BY_CF, conn);
                cmd.Parameters.Add(new SqlParameter("@CodiceFiscale", CodiceFiscale));
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var prenotazione = new Prenotazione
                    {
                        idPrenotazione = reader.GetInt32(0),
                        cliente = new Cliente
                        {
                            CodiceFiscale = reader.GetString(1),
                            Cognome = reader.GetString(2),
                            Nome = reader.GetString(3),
                            Citta = reader.GetString(4),
                            Provincia = reader.GetString(5),
                            Email = reader.GetString(6),
                            Telefono = reader.IsDBNull(7) ? null : reader.GetString(7),
                            Cellulare = reader.IsDBNull(8) ? null : reader.GetString(8),
                        },
                        camera = new Camera
                        {
                            Descrizione = reader.GetString(9),
                            Tipologia = reader.GetString(10),
                        },
                        DataPrenotazione = reader.GetDateTime(11),
                        InizioSoggiorno = reader.GetDateTime(12),
                        FineSoggiorno = reader.GetDateTime(13),
                        Caparra = reader.GetDecimal(14),
                        Tariffa = reader.GetDecimal(15),
                        TipoPensione = reader.IsDBNull(16) ? null : reader.GetString(16),
                        serviziPerPrenotazione = new List<ServizioPerPrenotazione>()
                    };

                    if (!reader.IsDBNull(17))
                    {
                        prenotazione.serviziPerPrenotazione.Add(new ServizioPerPrenotazione
                        {
                            idSP = reader.GetInt32(17),
                            Prezzo = reader.GetDecimal(18),
                            Quantita = reader.GetInt32(19),
                            DescrizioneServizio = reader.GetString(20)
                        });
                    }
                    prenotazioni.Add(prenotazione);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni tramite CF", ex);
            }
            return prenotazioni;
        }

        public IEnumerable<Prenotazione> GetPrenotazioneByPensione(string TipoPensione)
        {
            throw new NotImplementedException();
        }
    }
}
