using CoreFramework;
using Entity;
using IDal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DataDB
{
    public class PrenotazioneDAL : IPrenotazioneDAL
    {

        private readonly string _connectionString;

        public PrenotazioneDAL(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }


        //mapper
        private Prenotazione MapFromReader(SqlDataReader reader)
        {
            return new Prenotazione
            {
                IDPrenotazione = (int)reader["IDPrenotazione"],
                IDRistorante = (int)reader["IDRistorante"],
                NomeUtente = reader["NomeUtente"].ToString(),
                DataRichiesta = (DateTime)reader["DataRichiesta"],
                DataPrenotazione = (DateTime)reader["DataPrenotazione"],
                NumeroPersone = (int)reader["NumPersone"]
            };
        }



        public List<Prenotazione> GetByRistorante(int idRistorante)
        {
            var lista = new List<Prenotazione>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM Prenotazioni " +
                    "WHERE IDRistorante = @idRist " +
                    "ORDER BY DataPrenotazione", conn))
                {
                    cmd.Parameters.AddWithValue("@idRist", idRistorante);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel recupero delle prenotazioni.", ex);
            }
            return lista;
        }


        public List<Prenotazione> GetByRistoranteEData(int idRistorante, DateTime data)
        {
            var lista = new List<Prenotazione>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM Prenotazioni " +
                    "WHERE IDRistorante = @idRist AND " +
                    "CAST(DataPrenotazione AS DATE) = CAST(@data AS DATE) " +
                    "ORDER BY DataPrenotazione", conn))
                {
                    cmd.Parameters.AddWithValue("@idRist", idRistorante);
                    cmd.Parameters.AddWithValue("@data", data);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel recupero delle prenotazioni per data.", ex);
            }
            return lista;
        }

        public List<Prenotazione> GetByPeriodo(DateTime dal, DateTime al)
        {
            var lista = new List<Prenotazione>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM Prenotazioni " +
                    "WHERE DataPrenotazione >= @dal " +
                    "AND   DataPrenotazione <= @al " +
                    "ORDER BY DataPrenotazione", conn))
                {
                    cmd.Parameters.AddWithValue("@dal", dal);
                    cmd.Parameters.AddWithValue("@al", al);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel recupero delle prenotazioni per periodo.", ex);
            }
            return lista;
        }


        public Prenotazione GetById(int id)
        {
            Prenotazione prenotazione = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM Prenotazioni " +
                    "WHERE IDPrenotazione = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                            prenotazione = MapFromReader(reader);
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel recupero della prenotazione per ID.", ex);
            }
            return prenotazione;
        }


        public void Insert(Prenotazione p)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO Prenotazioni " +
                    "(IDRistorante, NomeUtente, DataRichiesta, " +
                    " DataPrenotazione, NumPersone) " +
                    "VALUES (@idRist, @nomeUtente, @dataRich, " +
                    "        @dataPren, @numPersone)", conn))
                {
                    cmd.Parameters.AddWithValue("@idRist", p.IDRistorante);
                    cmd.Parameters.AddWithValue("@nomeUtente", p.NomeUtente);
                    cmd.Parameters.AddWithValue("@dataRich", p.DataRichiesta);
                    cmd.Parameters.AddWithValue("@dataPren", p.DataPrenotazione);
                    cmd.Parameters.AddWithValue("@numPersone", p.NumeroPersone);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante l'inserimento della prenotazione.", ex);
            }
        }


        public void Update(Prenotazione p)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE Prenotazioni SET " +
                    "IDRistorante = @idRist, " +
                    "NomeUtente = @nomeUtente, " +
                    "DataPrenotazione = @dataPren, " +
                    "NumPersone = @numPersone " +
                    "WHERE IDPrenotazione = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", p.IDPrenotazione);
                    cmd.Parameters.AddWithValue("@idRist", p.IDRistorante);
                    cmd.Parameters.AddWithValue("@nomeUtente", p.NomeUtente);
                    cmd.Parameters.AddWithValue("@dataPren", p.DataPrenotazione);
                    cmd.Parameters.AddWithValue("@numPersone", p.NumeroPersone);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante la modifica della prenotazione.", ex);
            }
        }

        /////*
        ///System.Windows.Data Error: 40 : BindingExpression path error: 'NumPersone' property not found on 'object' 
        ///''Prenotazione' (HashCode=52845536)'. BindingExpression:Path=NumPersone; DataItem='Prenotazione' 
        ///(HashCode=52845536); target element is 'TextBlock' (Name=''); target property is 'Text' (type 'String')
        ///*/
        ///
        ///

        public void Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "DELETE FROM Prenotazioni WHERE IDPrenotazione = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore durante la cancellazione della prenotazione con ID {id}.", ex);
            }
        }

        public int GetTotalePrenotazioni(int? idRistorante, DateTime dal, DateTime al)
        {
            try
            {
                string query =
                    "SELECT COUNT(*) FROM Prenotazioni " +
                    "WHERE DataPrenotazione >= @dal " +
                    "AND   DataPrenotazione <= @al ";

                if (idRistorante.HasValue)
                    query += "AND IDRistorante = @idRist";

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dal", dal);
                    cmd.Parameters.AddWithValue("@al", al);
                    if (idRistorante.HasValue)
                        cmd.Parameters.AddWithValue("@idRist", idRistorante.Value);
                    conn.Open();
                    //scalar restituisce un singolo elemento
                    //cioe il count()
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel calcolo totale prenotazioni.", ex);
            }
        }

        public int GetTotalePersone(int? idRistorante, DateTime dal, DateTime al)
        {
            try
            {
                string query =
                    "SELECT ISNULL(SUM(NumPersone), 0) FROM Prenotazioni " +
                    "WHERE DataPrenotazione >= @dal " +
                    "AND   DataPrenotazione <= @al ";

                if (idRistorante.HasValue)
                    query += "AND IDRistorante = @idRist";

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dal", dal);
                    cmd.Parameters.AddWithValue("@al", al);
                    if (idRistorante.HasValue)
                        cmd.Parameters.AddWithValue("@idRist", idRistorante.Value);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel calcolo totale persone.", ex);
            }
        }

        public List<StatisticaGiornaliera> GetPerGiorno(
            int? idRistorante, DateTime dal, DateTime al)
        {
            var lista = new List<StatisticaGiornaliera>();

            try
            {
                string query =
                    "SELECT CAST(DataPrenotazione AS DATE) AS Giorno, " +
                    "COUNT(*) AS NumPrenotazioni, " +
                    "SUM(NumPersone) AS NumPersone " +
                    "FROM Prenotazioni " +
                    "WHERE DataPrenotazione >= @dal " +
                    "AND   DataPrenotazione <= @al ";

                if (idRistorante.HasValue)
                    query += "AND IDRistorante = @idRist ";

                query += "GROUP BY CAST(DataPrenotazione AS DATE) " +
                         "ORDER BY Giorno";

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dal", dal);
                    cmd.Parameters.AddWithValue("@al", al);
                    if (idRistorante.HasValue)
                        cmd.Parameters.AddWithValue("@idRist", idRistorante.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new StatisticaGiornaliera
                            {
                                Data = (DateTime)reader["Giorno"],
                                NumPrenotazioni = (int)reader["NumPrenotazioni"],
                                NumPersone = (int)reader["NumPersone"]
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore nel recupero statistiche per giorno.", ex);
            }

            return lista;
        }
    }
}
