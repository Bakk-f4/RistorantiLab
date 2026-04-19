using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace IDal
{
    public interface IUtenteDAL
    {
        List<Utente> GetAll();
        Utente GetByUserName(string username);
        void Insert(Utente utente);
        void Update(Utente utente);
        void Delete(string userName);

    }
}
