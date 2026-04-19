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
    public class PrenotazioneDetailViewModel : ViewModelBase
    {

        private readonly PrenotazioneManager _prenManager;

        private readonly Utente _utenteCorrente;

        private readonly bool _isNuova;
        private readonly int _idPrenotazione;


        public event EventHandler RichiestaTornaLista;

        //dati passati dal list alla detail
        public Ristorante RistoranteCorrente { get; }
        public List<Utente> ListaUtenti { get; }

        private List<Utente> _listaUtentiFiltrata;
        public List<Utente> ListaUtentiFiltrata
        {
            get => _listaUtentiFiltrata;
            set { _listaUtentiFiltrata = value; OnPropertyChanged(); }
        }



        //proprieta form
        private Utente _utenteSelezionato;
        public Utente UtenteSelezionato
        {
            get => _utenteSelezionato;
            set { _utenteSelezionato = value; OnPropertyChanged(); }
        }

        private DateTime _dataPrenotazione = DateTime.Today;
        public DateTime DataPrenotazione
        {
            get => _dataPrenotazione;
            set
            {
                _dataPrenotazione = value;
                OnPropertyChanged();
                //aggiorna i posti disponibili in tempo reale
                AggiornaPosti();
            }
        }

        private int _numPersone = 1;
        public int NumPersone
        {
            get => _numPersone;
            set
            {
                _numPersone = value;
                OnPropertyChanged();
            }
        }


        private int _postiDisponibili;
        public int PostiDisponibili
        {
            get => _postiDisponibili;
            set { _postiDisponibili = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }


        public bool IsAdministrator => _utenteCorrente?.isAdministrator ?? false;

        public string Titolo => _isNuova
            ? $"Nuova prenotazione — {RistoranteCorrente?.RagioneSociale}"
            : $"Modifica prenotazione — {RistoranteCorrente?.RagioneSociale}";




        //comandi
        public ICommand SalvaCommand { get; }
        public ICommand AnnullaCommand { get; }


        //costruttore
        public PrenotazioneDetailViewModel(
            PrenotazioneManager prenManager,
            RistoranteManager ristManager,
            UtenteManager utenteManager,
            Ristorante ristorante,
            Prenotazione prenotazione,
            Utente utenteCorrente)
        {
            _prenManager = prenManager ?? throw new ArgumentNullException(nameof(prenManager));
            _isNuova = prenotazione == null;
            _idPrenotazione = prenotazione?.IDPrenotazione ?? 0;
            RistoranteCorrente = ristorante;
            _utenteCorrente = utenteCorrente;

            //carica la lista utenti per il ComboBox
            try
            {
                ListaUtenti = utenteManager.GetAll();
                var tuttiGliUtenti = utenteManager.GetAll();

                // Utente normale → vede solo se stesso
                // Amministratore → vede tutti
                ListaUtentiFiltrata = utenteCorrente.isAdministrator
                    ? tuttiGliUtenti
                    : tuttiGliUtenti
                        .Where(u => u.Username == utenteCorrente.Username)
                        .ToList();
            }
            catch
            {
                ListaUtenti = new List<Utente>();
                ListaUtentiFiltrata = new List<Utente>();
            }

            if (!utenteCorrente.isAdministrator)
            {
                UtenteSelezionato = ListaUtentiFiltrata.FirstOrDefault();
            }

            //popola i campi se è una modifica
            if (!_isNuova)
            {
                DataPrenotazione = prenotazione.DataPrenotazione;
                NumPersone = prenotazione.NumeroPersone;

                //seleziona l'utente corrispondente nella lista
                UtenteSelezionato = ListaUtentiFiltrata
                    .Find(u => u.Username == prenotazione.NomeUtente);
            }

            SalvaCommand = new RelayCommand(EseguiSalva);
            AnnullaCommand = new RelayCommand(
                () => RichiestaTornaLista?.Invoke(this, EventArgs.Empty));

            //calcola i posti disponibili all'apertura
            AggiornaPosti();
        }

        private void AggiornaPosti()
        {
            if (RistoranteCorrente == null) return;

            try
            {
                PostiDisponibili = _prenManager.GetPostiDisponibili(
                    RistoranteCorrente.IDRistorante, DataPrenotazione);
            }
            catch (Exception ex)
            {
                MessaggioErrore = $"Errore durante il calcolo dei posti disponibili: {ex.Message}";
            }
        }

        private void EseguiSalva()
        {
            try
            {
                MessaggioErrore = null;

                var p = new Prenotazione
                {
                    IDPrenotazione = _idPrenotazione,
                    IDRistorante = RistoranteCorrente.IDRistorante,
                    NomeUtente = UtenteSelezionato?.Username,
                    DataPrenotazione = DataPrenotazione,
                    NumeroPersone = NumPersone
                };

                _prenManager.Salva(p, _isNuova, _utenteCorrente);
                RichiestaTornaLista?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

































    }
}
