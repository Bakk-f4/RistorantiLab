using Entity;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Helpers;

namespace UI.ViewModels
{
    public class RistoranteListViewModel : ViewModelBase
    {
        private readonly RistoranteManager _manager;

        // ─── proprietà in binding ────────────────────────────────────────

        private List<Ristorante> _listaRistoranti;
        public List<Ristorante> ListaRistoranti
        {
            get => _listaRistoranti;
            set
            {
                _listaRistoranti = value;
                OnPropertyChanged();
            }
        }

        private Ristorante _ristoranteSelezionato;
        public Ristorante RistoranteSelezionato
        {
            get => _ristoranteSelezionato;
            set
            {
                _ristoranteSelezionato = value;
                OnPropertyChanged();
            }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set
            {
                _messaggioErrore = value;
                OnPropertyChanged();
            }
        }

        // ─── comandi ─────────────────────────────────────────────────────
        public ICommand CaricaCommand { get; }

        // ─── costruttore ─────────────────────────────────────────────────
        public RistoranteListViewModel(RistoranteManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            CaricaCommand = new RelayCommand(EseguiCarica);

            // Carica i dati subito all'apertura della schermata
            EseguiCarica();
        }

        // ─── logica dei comandi ──────────────────────────────────────────
        private void EseguiCarica()
        {
            try
            {
                MessaggioErrore = null;
                ListaRistoranti = _manager.GetAll();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
                ListaRistoranti = new List<Ristorante>();
            }
        }
    }
}
