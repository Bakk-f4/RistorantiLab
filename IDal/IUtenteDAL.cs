using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDal
{
    public interface IUtenteDAL
    {
        List<Utente> GetAll();
        Utente GetByUserName(string userName);
        void Insert(Utente utente);
        void Update(Utente utente);
        void Delete(string userName);
    }
}
