using Entity;
using Manager;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Helpers;
using System.Windows;

namespace UI.ViewModels
{
    /// <summary>
    /// Classe principale che gestisce la navigazione tra le viste e coordina i ViewModel specifici.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        private readonly RistoranteManager _ristoranteManager;
        private readonly UtenteManager _utenteManager;
        private readonly PrenotazioneManager _prenotazioneManager;
        private readonly StatisticheManager _statisticheManager;



        //utente loggato
        private Utente _utenteCorrente;
        public Utente UtenteCorrente
        {
            get => _utenteCorrente;
            set
            {
                _utenteCorrente = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMenuVisibile));
                OnPropertyChanged(nameof(IsAmministrator));
            }
        }

        //menu laterale visibile solo dopo login
        public bool IsMenuVisibile => UtenteCorrente != null;


        //alcune voci visibili solo per amministratori
        //nasconde lista utenti a utenti non amministratori
        public bool IsAmministrator => UtenteCorrente?.isAdministrator ?? false;


        //voce menu attiva
        private string _sezioneAttiva;
        public string SezioneAttiva
        {
            get => _sezioneAttiva;
            set
            {
                _sezioneAttiva = value;
                OnPropertyChanged();
            }
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


        //comandi
        public ICommand VaiARistorantiCommand { get; }
        public ICommand VaiAUtentiCommand { get; }
        public ICommand VaiAPrenotazioniCommand { get; }
        public ICommand VaiAStatisticheCommand { get; }
        public ICommand LogoutCommand { get; }


        //costruttore
        public MainViewModel(
            RistoranteManager ristoranteManager,
            UtenteManager utenteManager,
            PrenotazioneManager prenotazioneManager,
            StatisticheManager statisticheManager,
            Utente utenteCorrente)
        {
            _ristoranteManager = ristoranteManager
                ?? throw new ArgumentNullException(nameof(ristoranteManager));
            _utenteManager = utenteManager
                ?? throw new ArgumentNullException(nameof(utenteManager));
            _prenotazioneManager = prenotazioneManager
                ?? throw new ArgumentNullException(nameof(prenotazioneManager));
            _statisticheManager = statisticheManager
                ?? throw new ArgumentNullException(nameof(statisticheManager));
            UtenteCorrente = utenteCorrente
                ?? throw new ArgumentNullException(nameof(utenteCorrente));

            //inizializzazione comandi
            VaiARistorantiCommand = new RelayCommand(MostraListaRistoranti);

            VaiAUtentiCommand = new RelayCommand(
                execute: MostraListaUtenti,
                canExecute: () => IsAmministrator);

            VaiAPrenotazioniCommand = new RelayCommand(
                execute: MostraListaPrenotazioni,
                canExecute: () => IsMenuVisibile);

            VaiAStatisticheCommand = new RelayCommand(
                execute: MostraStatistiche,
                canExecute: () => IsMenuVisibile);

            VaiAPrenotazioniCommand = new RelayCommand(
                execute: MostraListaPrenotazioni,
                canExecute: () => IsMenuVisibile);

            LogoutCommand = new RelayCommand(EseguiLogout);

            //vista iniziale
            MostraListaRistoranti();
        }

        private void MostraStatistiche()
        {
            SezioneAttiva = "Statistiche";
            CurrentView = new StatisticheViewModel(
                _statisticheManager,
                _ristoranteManager);
        }


        //Navigazione
        //private void MostraLogin()
        //{
        //    var vm = new LoginViewModel(_utenteManager);
        //    vm.LoginRiuscito += OnLoginRiuscito;
        //    CurrentView = vm;
        //}

        public void MostraListaRistoranti()
        {
            SezioneAttiva = "Ristoranti";
            var vm = new RistoranteListViewModel(_ristoranteManager, IsAmministrator);
            vm.RichiestaNuovo += (s, e) => MostraDettaglioRistorante(null);
            vm.RichiestaModifica += (s, r) => MostraDettaglioRistorante(r);
            CurrentView = vm;
        }

        private void MostraDettaglioRistorante(Ristorante r)
        {
            SezioneAttiva = "Ristoranti";
            var vm = new RistoranteDetailViewModel(_ristoranteManager, r);
            vm.RichiestaTornaLista += (s, e) => MostraListaRistoranti();
            CurrentView = vm;
        }

        public void MostraListaUtenti()
        {
            SezioneAttiva = "Utenti";
            var vm = new UtenteListViewModel(_utenteManager);
            vm.RichiestaNuovo += (s, e) => MostraDettaglioUtente(null);
            vm.RichiestaModifica += (s, u) => MostraDettaglioUtente(u);
            CurrentView = vm;
        }

        private void MostraDettaglioUtente(Utente u)
        {
            SezioneAttiva = "Utenti";
            var vm = new UtenteDetailViewModel(_utenteManager, u);
            vm.RichiestaTornaLista += (s, e) => MostraListaUtenti();
            CurrentView = vm;
        }

        private void MostraListaPrenotazioni()
        {
            SezioneAttiva = "Prenotazioni";
            var vm = new PrenotazioneListViewModel(
                _prenotazioneManager, _ristoranteManager, UtenteCorrente);

            vm.RichiestaNuova += (s, e) =>
                MostraDettaglioPrenotazione(null,
                    (vm as PrenotazioneListViewModel)?.RistoranteSelezionato);

            vm.RichiestaModifica += (s, p) =>
                MostraDettaglioPrenotazione(p,
                    (vm as PrenotazioneListViewModel)?.RistoranteSelezionato);

            CurrentView = vm;
        }

        private void MostraDettaglioPrenotazione(Prenotazione prenotazione, Ristorante ristorante)
        {

            SezioneAttiva = "Prenotazioni";

            //istanzio il dettaglio passando la prenotazione (null se nuova)
            //e il ristorante selezionato nella lista
            var vm = new PrenotazioneDetailViewModel(
                _prenotazioneManager,
                _ristoranteManager,
                _utenteManager,
                ristorante,
                prenotazione,
                UtenteCorrente);

            vm.RichiestaTornaLista += (s, e) => MostraListaPrenotazioni();
            CurrentView = vm;
        }

        //resetto l' utente corrente e torno alla login
        private void EseguiLogout()
        {
            UtenteCorrente = null;

            System.Diagnostics.Process.Start(
            Application.ResourceAssembly.Location);
            Application.Current.Shutdown();

            //MostraLogin();

        }


        //handler login
        //private void OnLoginRiuscito(object sender, Utente utente)
        //{
        //    UtenteCorrente = utente;
        //    MostraListaRistoranti();
        //}



        public void MostraLista()
        {
            var vm = new RistoranteListViewModel(_ristoranteManager, IsAmministrator);

            //Sottoscrive gli eventi di navigazione dalla lista
            vm.RichiestaNuovo += OnRichiestaNuovo;
            vm.RichiestaModifica += OnRichiestaModifica;

            CurrentView = vm;
        }



        private void MostraDettaglio(Ristorante ristorante)
        {
            var vm = new RistoranteDetailViewModel(
                _ristoranteManager, ristorante);

            //Sottoscrive gli eventi di navigazione dal dettaglio
            vm.RichiestaTornaLista += OnRichiestaTornaLista;

            CurrentView = vm;
        }


        //Handler eventi di navigazione
        private void OnRichiestaNuovo(object sender, EventArgs e)
            => MostraDettaglio(null);

        private void OnRichiestaModifica(object sender, Ristorante r)
            => MostraDettaglio(r);

        private void OnRichiestaTornaLista(object sender, EventArgs e)
            => MostraLista();

    }
}
