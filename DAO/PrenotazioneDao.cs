using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Software_hotel.Models;
using Software_hotel.Services;

namespace Software_hotel.DAO
{
    public class PrenotazioneDao : SqlServiceSvcBase, IPrenontazioneDao
    {
        private const string SELECT_ALL_PRENOTAZIONI = @"
        WITH TotaleServizi AS (
        SELECT idPrenotazione, SUM(Prezzo * Quantita) AS Totale
        FROM ServiziPerPrenotazione
        GROUP BY idPrenotazione)
        SELECT p.idPrenotazione, c.CodiceFiscale, c.Cognome, c.Nome, c.Citta, c.Provincia, c.Email, c.Telefono, c.Cellulare,
        ca.Descrizione AS DescrizioneStanza, ca.Tipologia,
        p.DataPrenotazione, p.DataInizioSoggiorno, p.DataFineSoggiorno, p.Caparra, p.Tariffa, p.TipoPensione,
        t.Totale AS TotaleServizi
        FROM Prenotazioni p
        INNER JOIN Clienti c ON p.idCliente = c.idCliente
        INNER JOIN Camera ca ON p.idCamera = ca.idCamera
        LEFT JOIN TotaleServizi t ON p.idPrenotazione = t.idPrenotazione
        ";


        private const string SELECT_BY_CF = @"
        SELECT p.idPrenotazione, c.CodiceFiscale, c.Cognome, c.Nome, c.Citta, c.Provincia, c.Email, c.Telefono, c.Cellulare,
        ca.Descrizione AS DescrizioneStanza, ca.Tipologia,
        p.DataPrenotazione, p.DataInizioSoggiorno, p.DataFineSoggiorno, p.Caparra, p.Tariffa, p.TipoPensione       
        FROM Prenotazioni p
        INNER JOIN Clienti c ON p.idCliente = c.idCliente
        INNER JOIN Camera ca ON p.idCamera = ca.idCamera
        WHERE c.CodiceFiscale LIKE @CodiceFiscale + '%'";

        private const string SELECT_BY_PENSIONE = @"
        SELECT p.idPrenotazione, c.CodiceFiscale, c.Cognome, c.Nome, c.Citta, c.Provincia, c.Email, c.Telefono, c.Cellulare,
        ca.Descrizione AS DescrizioneStanza, ca.Tipologia,
        p.DataPrenotazione, p.DataInizioSoggiorno, p.DataFineSoggiorno, p.Caparra, p.Tariffa, p.TipoPensione       
        FROM Prenotazioni p
        INNER JOIN Clienti c ON p.idCliente = c.idCliente
        INNER JOIN Camera ca ON p.idCamera = ca.idCamera
        WHERE p.TipoPensione = 'pensione completa'";

        private const string SELECT_ALL_SERVIZI = @"
        SELECT IdServizio, DescrizioneServizio FROM ServiziAggiuntivi";

        private const string ADD_SERVIZIO = @"
        INSERT INTO ServiziPerPrenotazione (idServizio, idPrenotazione, Prezzo, Quantita)
        VALUES (@idServizio, @idPrenotazione, @Prezzo, @Quantita);";

        private const string TOTALE_CON_SERVIZIO = @"
        WITH TotaleServizi AS (SELECT idPrenotazione, SUM(Prezzo * Quantita) AS Totale
        FROM ServiziPerPrenotazione GROUP BY idPrenotazione)
        SELECT p.Tariffa - p.Caparra + Totale FROM Prenotazioni p JOIN TotaleServizi t ON p.idPrenotazione = t.idPrenotazione
        WHERE p.idPrenotazione = @idPrenotazione";

        private const string SELECT_SERVIZI_PER_PRENOTAZIONE = @"
        SELECT sp.idSp, sp.idServizio, sp.idPrenotazione, sp.Prezzo, sp.Quantita,
        sa.DescrizioneServizio
        FROM ServiziPerPrenotazione sp
        JOIN ServiziAggiuntivi sa ON sp.idServizio = sa.idServizio 
        WHERE idPrenotazione = @id";
        public PrenotazioneDao(IConfiguration config) : base(config)
        {
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
                    decimal totaleServizi = reader.IsDBNull(17) ? 0 : reader.GetDecimal(17); 

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
                        ImportoDaSaldare = reader.GetDecimal(15) - reader.GetDecimal(14) + totaleServizi 
                    };
                    prenotazioni.Add(prenotazione);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni", ex);
            }

            return prenotazioni;
        }



        public IEnumerable<ServiziAggiuntivi> GetListaServizi()
        {
            var servizi = new List<ServiziAggiuntivi>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_ALL_SERVIZI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var servizio = new ServiziAggiuntivi
                    {
                        idServizio = reader.GetInt32(0),
                        DescrizioneServizio = reader.GetString(1)
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

        public decimal AggiungiServizio(int idPrenotazione, ServizioPerPrenotazione servizioPerPrenotazione)
        {
            decimal importoDaSaldare = 0;

            try
            {
                using var conn = CreateConnection();
                conn.Open();

                using var cmd = GetCommand(ADD_SERVIZIO, conn);
                cmd.Parameters.Add(new SqlParameter("@idServizio", servizioPerPrenotazione.idServizio));
                cmd.Parameters.Add(new SqlParameter("@idPrenotazione", idPrenotazione));
                cmd.Parameters.Add(new SqlParameter("@Prezzo", servizioPerPrenotazione.Prezzo));
                cmd.Parameters.Add(new SqlParameter("@Quantita", servizioPerPrenotazione.Quantita));
                cmd.ExecuteNonQuery();

                // SALDO CON SERVIZIO AGGIUNTIVO
                using (var cmdTotal = GetCommand(TOTALE_CON_SERVIZIO, conn))
                {
                    cmdTotal.Parameters.Add(new SqlParameter("@idPrenotazione", idPrenotazione));

                    using var reader = cmdTotal.ExecuteReader();
                    if (reader.Read())
                    {
                        importoDaSaldare = reader.GetDecimal(0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiunta del servizio e nel calcolo dell'importo da saldare", ex);
            }
            return importoDaSaldare;
        }


        public IEnumerable<ServizioPerPrenotazione> GetServiziPerPrenotazione(int idPrenotazione)
        {
            List<ServizioPerPrenotazione> servizi = new List<ServizioPerPrenotazione>();

            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_SERVIZI_PER_PRENOTAZIONE, conn);
                cmd.Parameters.Add(new SqlParameter("@id", idPrenotazione));

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
                throw new Exception("Errore", ex);
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

                    decimal totaleServizi = 0m; 

                    prenotazione.ImportoDaSaldare = prenotazione.Tariffa - prenotazione.Caparra + totaleServizi;
                    prenotazioni.Add(prenotazione);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni", ex);
            }
            return prenotazioni;
        }

        public IEnumerable<Prenotazione> GetPrenotazioneByPensione(string TipoPensione)
        {
            var prenotazioni = new List<Prenotazione>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_BY_PENSIONE, conn);
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
                        TipoPensione = reader.GetString(16)
                    };
                    prenotazioni.Add(prenotazione);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni per pensione completa", ex);
            }

            return prenotazioni;
        }
    }
}
