using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Utente
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool isAdministrator { get; set; } = false;
        public string Descrizione { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Citta { get; set; }
    }
}
