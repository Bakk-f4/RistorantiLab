using CoreFramework;
using Entity;
using IDal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDB
{
    /// <summary>
    /// Classe che implementa l'interfaccia IRistoranteDAL per accedere 
    /// ai dati dei ristoranti memorizzati in un database SQL Server.
    /// </summary>
    public class RistoranteDAL : IRistoranteDAL
    {
        private readonly string _connectionString;
        public RistoranteDAL(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }


        /// <summary>
        /// Trasforma una riga (del DB) del SqlDataReader in un oggetto Ristorante.
        /// Chiamato da tutti i metodi Get — scritto una volta sola.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Ristorante MapFromReader(SqlDataReader reader)
        {
            return new Ristorante
            {
                IDRistorante = (int)reader["IDRistorante"],
                RagioneSociale = reader["RagioneSociale"].ToString(),
                PartitaIVA = reader["PartitaIVA"] == DBNull.Value
                                     ? null
                                     : reader["PartitaIVA"].ToString(),
                Indirizzo = reader["Indirizzo"] == DBNull.Value
                                     ? null
                                     : reader["Indirizzo"].ToString(),
                Citta = reader["Citta"] == DBNull.Value
                                     ? null
                                     : reader["Citta"].ToString(),
                Telefono = reader["Telefono"] == DBNull.Value
                                     ? null
                                     : reader["Telefono"].ToString(),
                Tipologia = (TipologiaRistorante)(int)reader["Tipologia"],
                NumPosti = (int)reader["NumPosti"],
                PrezzoMedio = reader["PrezzoMedio"] == DBNull.Value
                                     ? 0m
                                     : (decimal)reader["PrezzoMedio"]
            };
        }

        /// <summary>
        /// Restituisce una lista di tutti i ristoranti presenti nel database, ordinati per ragione sociale.
        /// </summary>
        /// <returns>Lista di <see cref="Ristorante"/> oggetti che rappresentano i ristoranti. 
        /// La lista è vuota se non vengono trovati ristoranti.</returns>
        /// <exception cref="BusinessException">Lanciato quando si verifica un errore </exception>
        public List<Ristorante> GetAll()
        {
            var lista = new List<Ristorante>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti ORDER BY RagioneSociale", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException("Errore durante recupero dati ristoranti.", ex);
            }
            return lista;
        }

        /// <summary>
        /// Restituisce il ristorante con l'ID specificato. 
        /// Se non viene trovato alcun ristorante con quell'ID, restituisce null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public Ristorante GetById(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti WHERE IDRistorante = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapFromReader(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore nel recupero del ristorante con ID {id}.", ex);
            }

            return null;
        }

        /// <summary>
        /// Restituisce i ristoranti che si trovano nella città specificata. 
        /// La ricerca è case-insensitive e permette di usare il carattere jolly % per trovare 
        /// ristoranti in città che contengono la stringa specificata. 
        /// I risultati sono ordinati per ragione sociale. Se non vengono trovati ristoranti, restituisce una lista vuota.
        /// </summary>
        /// <param name="citta"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public List<Ristorante> GetByCitta(string citta)
        {
            var lista = new List<Ristorante>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti " +
                    "WHERE Citta LIKE @citta " +
                    "ORDER BY RagioneSociale",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@citta", $"%{citta}%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nella ricerca per città.", ex);
            }

            return lista;
        }


        /// <summary>
        /// Restituisce i ristoranti che appartengono alla tipologia specificata.
        /// Se non vengono trovati ristoranti, restituisce una lista vuota.
        /// </summary>
        /// <param name="tipologia"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public List<Ristorante> GetByTipologia(TipologiaRistorante tipologia)
        {
            var lista = new List<Ristorante>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti " +
                    "WHERE Tipologia = @tipologia " +
                    "ORDER BY RagioneSociale",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@tipologia", (int)tipologia);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nella ricerca per tipologia.", ex);
            }

            return lista;
        }

        /// <summary>
        /// Restituisce i ristoranti che hanno un prezzo medio minore o uguale a quello specificato.
        /// Se non vengono trovati ristoranti, restituisce una lista vuota.
        /// </summary>
        /// <param name="prezzoMax"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public List<Ristorante> GetByPrezzo(decimal prezzoMax)
        {
            var lista = new List<Ristorante>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti " +
                    "WHERE PrezzoMedio <= @prezzoMax " +
                    "ORDER BY PrezzoMedio",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@prezzoMax", prezzoMax);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nella ricerca per prezzo.", ex);
            }

            return lista;
        }

        /// <summary>
        /// Inserisce un nuovo ristorante nel database. L'ID del ristorante viene generato automaticamente dal database.
        /// </summary>
        /// <param name="ristorante"></param>
        /// <exception cref="BusinessException"></exception>
        public void Insert(Ristorante ristorante)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO AnagraficaRistoranti " +
                    "(RagioneSociale, PartitaIVA, Indirizzo, Citta, " +
                    " Telefono, Tipologia, NumPosti, PrezzoMedio) " +
                    "VALUES (@ragione, @piva, @indirizzo, @citta, " +
                    "        @telefono, @tipologia, @posti, @prezzo)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@ragione", ristorante.RagioneSociale);
                    cmd.Parameters.AddWithValue("@piva",
                        (object)ristorante.PartitaIVA ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@indirizzo",
                        (object)ristorante.Indirizzo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta",
                        (object)ristorante.Citta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono",
                        (object)ristorante.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipologia", (int)ristorante.Tipologia);
                    cmd.Parameters.AddWithValue("@posti", ristorante.NumPosti);
                    cmd.Parameters.AddWithValue("@prezzo",
                        (object)ristorante.PrezzoMedio == null
                            ? DBNull.Value
                            : (object)ristorante.PrezzoMedio);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante l'inserimento del ristorante.", ex);
            }
        }

        /// <summary>
        /// Aggiorna le informazioni di un ristorante esistente nel database. 
        /// Il ristorante da aggiornare viene identificato tramite l'ID presente nell'oggetto Ristorante passato come parametro.
        /// </summary>
        /// <param name="ristorante"></param>
        /// <exception cref="BusinessException"></exception>
        public void Update(Ristorante ristorante)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE AnagraficaRistoranti SET " +
                    "   RagioneSociale = @ragione, " +
                    "   PartitaIVA     = @piva, " +
                    "   Indirizzo      = @indirizzo, " +
                    "   Citta          = @citta, " +
                    "   Telefono       = @telefono, " +
                    "   Tipologia      = @tipologia, " +
                    "   NumPosti       = @posti, " +
                    "   PrezzoMedio    = @prezzo " +
                    "WHERE IDRistorante = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", ristorante.IDRistorante);
                    cmd.Parameters.AddWithValue("@ragione", ristorante.RagioneSociale);
                    cmd.Parameters.AddWithValue("@piva",
                        (object)ristorante.PartitaIVA ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@indirizzo",
                        (object)ristorante.Indirizzo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta",
                        (object)ristorante.Citta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono",
                        (object)ristorante.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipologia", (int)ristorante.Tipologia);
                    cmd.Parameters.AddWithValue("@posti", ristorante.NumPosti);
                    cmd.Parameters.AddWithValue("@prezzo",
                        (object)ristorante.PrezzoMedio == null
                            ? DBNull.Value
                            : (object)ristorante.PrezzoMedio);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore durante la modifica del ristorante {ristorante.RagioneSociale}.", ex);
            }
        }

        /// <summary>
        /// Cancella il ristorante con l'ID specificato dal database.
        /// Se non viene trovato alcun ristorante con quell'ID, non viene eseguita alcuna operazione.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="BusinessException"></exception>
        public void Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "DELETE FROM AnagraficaRistoranti WHERE IDRistorante = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore durante la cancellazione del ristorante con ID {id}.", ex);
            }
        }

        public List<Ristorante> Cerca(
            string citta,
            TipologiaRistorante? tipologia,
            decimal? prezzoMax)
        {

            var lista = new List<Ristorante>();

            try
            {

                var condizioni = new List<string>();
                var query = "SELECT * FROM AnagraficaRistoranti";

                if (!string.IsNullOrWhiteSpace(citta))
                    condizioni.Add("Citta LIKE @citta");

                if (tipologia.HasValue)
                    condizioni.Add("Tipologia = @tipologia");

                if (prezzoMax.HasValue && prezzoMax.Value > 0)
                    condizioni.Add("PrezzoMedio <= @prezzoMax");

                // Aggiunge WHERE solo se almeno una condizione è attiva
                if (condizioni.Count > 0)
                    query += " WHERE " + string.Join(" AND ", condizioni);

                query += " ORDER BY RagioneSociale";

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    // Aggiunge i parametri solo per i filtri attivi
                    if (!string.IsNullOrWhiteSpace(citta))
                        cmd.Parameters.AddWithValue("@citta", $"%{citta}%");

                    if (tipologia.HasValue)
                        cmd.Parameters.AddWithValue(
                            "@tipologia", (int)tipologia.Value);

                    if (prezzoMax.HasValue && prezzoMax.Value > 0)
                        cmd.Parameters.AddWithValue("@prezzoMax", prezzoMax.Value);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                }
            }
            catch (SqlException ex)
            {

                throw new BusinessException(
                    $"Errore durante la ricerca dei ristoranti.", ex);
            }
            return lista;
        }

        // TODO: sistemare le query, in modo tale che se faccio delete su un ristorante, cancelli anche tutte le prenotazioni collegate a quel ristorante (cascata)
        // TODO: quando clicko su una prenotazione, impostare la data DataFiltro con DataPrenotazione della prentazione selezionata


    }
}
