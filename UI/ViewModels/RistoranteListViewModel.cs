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

        //eventi di navigazione 
        public event EventHandler RichiestaNuovo;
        public event EventHandler<Ristorante> RichiestaModifica;


        //proprietà in binding

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

        private string _messaggioSuccesso;
        public string MessaggioSuccesso
        {
            get => _messaggioSuccesso;
            set { _messaggioSuccesso = value; OnPropertyChanged(); }
        }

        //comandi
        public ICommand CaricaCommand { get; }
        public ICommand NuovoCommand { get; }
        public ICommand ModificaCommand { get; }
        public ICommand EliminaCommand { get; }


        public RistoranteListViewModel(RistoranteManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            CaricaCommand = new RelayCommand(EseguiCarica);
            NuovoCommand = new RelayCommand(EseguiNuovo);
            ModificaCommand = new RelayCommand(
                execute: EseguiModifica,
                canExecute: () => RistoranteSelezionato != null);
            EliminaCommand = new RelayCommand(
                execute: EseguiElimina,
                canExecute: () => RistoranteSelezionato != null);

            EseguiCarica();
        }

        //logica dei comandi
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
        private void EseguiNuovo()
            => RichiestaNuovo?.Invoke(this, EventArgs.Empty);

        private void EseguiModifica()
            => RichiestaModifica?.Invoke(this, RistoranteSelezionato);

        private void EseguiElimina()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;
                _manager.Elimina(RistoranteSelezionato.IDRistorante);
                MessaggioSuccesso = $"Ristorante '" +
                    $"{RistoranteSelezionato.RagioneSociale}" +
                    $"' eliminato con successo.";
                EseguiCarica();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }
    }
}
