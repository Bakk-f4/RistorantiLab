using Entity;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Helpers;

namespace UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private readonly RistoranteManager _ristoranteManager;
        private readonly UtenteManager _utenteManager;

        //utente loggato
        private Utente _utenteCorrente;
        public Utente UtenteCorrente
        {
            get => _utenteCorrente;
            set { _utenteCorrente = value; OnPropertyChanged(); }
        }


        //vista corrente
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(RistoranteManager ristoranteManager, UtenteManager utenteManager)
        {
            _ristoranteManager = ristoranteManager
                ?? throw new ArgumentNullException(nameof(ristoranteManager));
            _utenteManager = utenteManager
                ?? throw new ArgumentNullException(nameof(utenteManager));

            //parte dalla schermata di login
            MostraLogin();
        }


        //navigazione
        private void MostraLogin()
        {
            var vm = new LoginViewModel(_utenteManager);
            vm.LoginRiuscito += OnLoginRiuscito;
            CurrentView = vm;
        }

        public void MostraListaRistoranti()
        {
            var vm = new RistoranteListViewModel(_ristoranteManager);
            vm.RichiestaNuovo += (s, e) => MostraDettaglioRistorante(null);
            vm.RichiestaModifica += (s, r) => MostraDettaglioRistorante(r);
            CurrentView = vm;
        }

        private void MostraDettaglioRistorante(Ristorante r)
        {
            var vm = new RistoranteDetailViewModel(_ristoranteManager, r);
            vm.RichiestaTornaLista += (s, e) => MostraListaRistoranti();
            CurrentView = vm;
        }

        public void MostraListaUtenti()
        {
            var vm = new UtenteListViewModel(_utenteManager);
            vm.RichiestaNuovo += (s, e) => MostraDettaglioUtente(null);
            vm.RichiestaModifica += (s, u) => MostraDettaglioUtente(u);
            CurrentView = vm;
        }

        private void MostraDettaglioUtente(Utente u)
        {
            var vm = new UtenteDetailViewModel(_utenteManager, u);
            vm.RichiestaTornaLista += (s, e) => MostraListaUtenti();
            CurrentView = vm;
        }

        //handler login
        private void OnLoginRiuscito(object sender, Utente utente)
        {
            UtenteCorrente = utente;
            MostraListaRistoranti();
        }





        public void MostraLista()
        {
            var vm = new RistoranteListViewModel(_ristoranteManager);

            //sottoscrive gli eventi di navigazione dalla lista
            vm.RichiestaNuovo += OnRichiestanuovo;
            vm.RichiestaModifica += OnRichiestaModifica;

            CurrentView = vm;
        }

        private void MostraDettaglio(Ristorante ristorante)
        {
            var vm = new RistoranteDetailViewModel(
                _ristoranteManager, ristorante);

            //sottoscrive gli eventi di navigazione dal dettaglio
            vm.RichiestaTornaLista += OnRichiestaTornaLista;

            CurrentView = vm;
        }

        private void OnRichiestanuovo(object sender, EventArgs e)
            => MostraDettaglio(null);

        private void OnRichiestaModifica(object sender, Ristorante r)
            => MostraDettaglio(r);

        private void OnRichiestaTornaLista(object sender, EventArgs e)
            => MostraLista();


    }
}
