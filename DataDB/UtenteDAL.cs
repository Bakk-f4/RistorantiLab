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
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }


        private Utente MapFromReader(SqlDataReader reader)
        {
            return new Utente
            {
                Username = reader["Username"].ToString(),
                PasswordHash = reader["Password"].ToString(),
                isAdministrator = (bool)reader["isAdministrator"],
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
                using (var cmd = new SqlCommand("SELECT * FROM Utenti ORDER BY Username", conn))
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
                throw new BusinessException("Errore durante recupero di tutti gli utenti.", ex);
            }
            return lista;
        }


        public Utente GetByUserName(string userName)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SELECT * FROM Utenti WHERE Username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", userName);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

                throw new BusinessException($"Errore durante recupero utente dal nome '{userName}'.", ex);
            }
            return null;
        }

        public void Insert(Utente utente)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO Utenti" +
                    " (Username, Password, isAdministrator, " +
                    "Descrizione, Telefono, Email, Citta) " +
                    "VALUES (@username, @password, @isAdmin, " +
                    "@descrizione, @telefono, @email, @citta)", conn))
                {
                    cmd.Parameters.AddWithValue("@username", utente.Username);
                    cmd.Parameters.AddWithValue("@password", utente.PasswordHash);
                    cmd.Parameters.AddWithValue("@isAdmin", utente.isAdministrator);
                    cmd.Parameters.AddWithValue("@descrizione", (object)utente.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono", (object)utente.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object)utente.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta", (object)utente.Citta ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException("Errore durante inserimento utente.", ex);
            }
        }



        public void Update(Utente utente)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("UPDATE Utenti SET " +
                    "Password           = @password, " +
                    "isAdministrator    = @isAdmin, " +
                    "Descrizione        = @descrizione, " +
                    "Telefono           = @telefono, " +
                    "Email              = @email, " +
                    "Citta              = @citta " +
                    "WHERE Username     = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", utente.Username);
                    cmd.Parameters.AddWithValue("@password", utente.PasswordHash);
                    cmd.Parameters.AddWithValue("@isAdmin", utente.isAdministrator);
                    cmd.Parameters.AddWithValue("@descrizione", (object)utente.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telefono", (object)utente.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object)utente.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@citta", (object)utente.Citta ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException($"Errore durante aggiornamento utente {utente.Username}.", ex);
            }
        }


        public void Delete(string userName)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(
                    "DELETE FROM Utenti WHERE Username = @username",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@username", userName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException($"Errore durante eliminazione utente {userName}.", ex);
            }
        }

    }
}
