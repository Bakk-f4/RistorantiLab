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
    public class UtenteDAL : IUtenteDAL
    {
        private readonly string _connectionString;

        public UtenteDAL(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        //mapping
        private Utente MapFromReader(SqlDataReader reader)
        {
            return new Utente
            {
                UserName = reader["UserName"].ToString(),
                PasswordHash = reader["Password"].ToString(),
                IsAdministrator = (bool)reader["IsAdministrator"],
                Descrizione = reader["Descrizione"] == DBNull.Value
                                      ? null
                                      : reader["Descrizione"].ToString(),
                Telefono = reader["Telefono"] == DBNull.Value
                                      ? null
                                      : reader["Telefono"].ToString(),
                Email = reader["Email"] == DBNull.Value
                                      ? null
                                      : reader["Email"].ToString(),
                Citta = reader["Citta"] == DBNull.Value
                                      ? null
                                      : reader["Citta"].ToString()
            };
        }
        public List<Utente> GetAll()
        {
            var lista = new List<Utente>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM dbo.Utenti ORDER BY UserName", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            lista.Add(MapFromReader(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante il recupero degli utenti.", ex);
            }

            return lista;
        }

        public Utente GetByUserName(string userName)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT * FROM dbo.Utenti WHERE UserName = @userName",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                            return MapFromReader(reader);
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore nel recupero dell'utente '{userName}'.", ex);
            }

            return null;
        }

        public void Insert(Utente utente)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.Utenti " +
                    "(UserName, Password, IsAdministrator, " +
                    " Descrizione, Telefono, Email, Citta) " +
                    "VALUES (@userName, @password, @isAdmin, " +
                    "        @descrizione, @telefono, @email, @citta)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@userName", utente.UserName);
                    cmd.Parameters.AddWithValue("@password", utente.PasswordHash);
                    cmd.Parameters.AddWithValue("@isAdmin", utente.IsAdministrator);
                    cmd.Parameters.AddWithValue("@descrizione",
                        (object)utente.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono",
                        (object)utente.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email",
                        (object)utente.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta",
                        (object)utente.Citta ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante l'inserimento dell'utente.", ex);
            }
        }

        public void Update(Utente utente)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.Utenti SET " +
                    "   Password        = @password, " +
                    "   IsAdministrator = @isAdmin, " +
                    "   Descrizione     = @descrizione, " +
                    "   Telefono        = @telefono, " +
                    "   Email           = @email, " +
                    "   Citta           = @citta " +
                    "WHERE UserName = @userName",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@userName", utente.UserName);
                    cmd.Parameters.AddWithValue("@password", utente.PasswordHash);
                    cmd.Parameters.AddWithValue("@isAdmin", utente.IsAdministrator);
                    cmd.Parameters.AddWithValue("@descrizione",
                        (object)utente.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono",
                        (object)utente.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email",
                        (object)utente.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta",
                        (object)utente.Citta ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    "Errore durante la modifica dell'utente.", ex);
            }
        }

        public void Delete(string userName)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "DELETE FROM dbo.Utenti WHERE UserName = @userName",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(
                    $"Errore durante la cancellazione dell'utente '{userName}'.", ex);
            }
        }
    }
}
