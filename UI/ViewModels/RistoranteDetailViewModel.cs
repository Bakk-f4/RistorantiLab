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
    public class RistoranteDetailViewModel : ViewModelBase
    {

        private readonly RistoranteManager _manager;
        private readonly bool _isNuovo;


        //evento di navigazione
        public event EventHandler RichiestaTornaLista;


        //proprieta oggetto in binding
        private Ristorante _ristorante;
        public Ristorante Ristorante
        {
            get => _ristorante;
            set { _ristorante = value; OnPropertyChanged(); }
        }


        //proprieta singole dell' oggetto. binding bidirezionale
        private string _ragioneSociale;
        public string RagioneSociale
        {
            get => _ragioneSociale;
            set { _ragioneSociale = value; OnPropertyChanged(); }
        }

        private string _partitaIVA;
        public string PartitaIVA
        {
            get => _partitaIVA;
            set { _partitaIVA = value; OnPropertyChanged(); }
        }

        private string _indirizzo;
        public string Indirizzo
        {
            get => _indirizzo;
            set { _indirizzo = value; OnPropertyChanged(); }
        }

        private string _citta;
        public string Citta
        {
            get => _citta;
            set { _citta = value; OnPropertyChanged(); }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        private TipologiaRistorante _tipologia;
        public TipologiaRistorante Tipologia
        {
            get => _tipologia;
            set { _tipologia = value; OnPropertyChanged(); }
        }

        private int _numPosti;
        public int NumPosti
        {
            get => _numPosti;
            set { _numPosti = value; OnPropertyChanged(); }
        }

        private decimal _prezzoMedio;
        public decimal PrezzoMedio
        {
            get => _prezzoMedio;
            set { _prezzoMedio = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }

        //titolo dinamico
        public string Titolo => _isNuovo
            ? "Nuovo ristorante"
            : $"Modifica — {RagioneSociale}";

        //lista tipologie per combobox
        public Array TipologieDisponibili
            => Enum.GetValues(typeof(TipologiaRistorante));


        //comandi
        public ICommand SalvaCommand { get; }
        public ICommand AnnullaCommand { get; }


        public RistoranteDetailViewModel(
            RistoranteManager manager,
            Ristorante ristorante)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            _isNuovo = ristorante == null;

            //Se e' nuovo inizializza con valori di default,
            //altrimenti popola i campi con i dati esistenti
            if (_isNuovo)
            {
                RagioneSociale = string.Empty;
                Tipologia = TipologiaRistorante.Pizzeria;
                NumPosti = 0;
                PrezzoMedio = 0m;
            }
            else
            {
                RagioneSociale = ristorante.RagioneSociale;
                PartitaIVA = ristorante.PartitaIVA;
                Indirizzo = ristorante.Indirizzo;
                Citta = ristorante.Citta;
                Telefono = ristorante.Telefono;
                Tipologia = ristorante.Tipologia;
                NumPosti = ristorante.NumPosti;
                PrezzoMedio = ristorante.PrezzoMedio;

                //Conserva il riferimento per l'ID in fase di Update
                Ristorante = ristorante;
            }

            SalvaCommand = new RelayCommand(EseguiSalva);
            AnnullaCommand = new RelayCommand(EseguiAnnulla);
        }


        //metodi per i comandi
        private void EseguiSalva()
        {
            try
            {
                MessaggioErrore = null;

                //ricostruisce l'oggetto dai campi del form
                var r = new Ristorante
                {
                    IDRistorante = _isNuovo ? 0 : Ristorante.IDRistorante,
                    RagioneSociale = RagioneSociale,
                    PartitaIVA = PartitaIVA,
                    Indirizzo = Indirizzo,
                    Citta = Citta,
                    Telefono = Telefono,
                    Tipologia = Tipologia,
                    NumPosti = NumPosti,
                    PrezzoMedio = PrezzoMedio
                };

                _manager.Salva(r);
               
                //torna alla lista dopo il salvataggio
                RichiestaTornaLista?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

        private void EseguiAnnulla()
            => RichiestaTornaLista?.Invoke(this, EventArgs.Empty);



    }
}
