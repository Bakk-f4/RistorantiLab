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
    public class RistoranteDAL : IRistoranteDAL
    {
        private readonly string _connectionString;

        public RistoranteDAL(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }


        // Trasforma una riga del SqlDataReader in un oggetto Ristorante.
        // Chiamato da tutti i metodi Get — scritto una volta sola.
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

        public List<Ristorante> GetAll()
        {
            var lista = new List<Ristorante>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM AnagraficaRistoranti ORDER BY RagioneSociale",
                    conn))
                {
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
                    "Errore durante il recupero dei ristoranti.", ex);
            }

            return lista;
        }

        
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
                    "Errore durante la modifica del ristorante.", ex);
            }
        }

        
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
    }
}
