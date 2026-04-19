using Entity;
using LiveCharts;
using LiveCharts.Wpf;
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
    public class StatisticheViewModel : ViewModelBase
    {

        private readonly StatisticheManager _statManager;
        private readonly RistoranteManager _ristManager;

        private bool _graficoInizializzato = false;



        //proprieta per filtrare
        private List<Ristorante> _listaRistoranti;
        public List<Ristorante> ListaRistoranti
        {
            get => _listaRistoranti;
            set { _listaRistoranti = value; OnPropertyChanged(); }
        }

        private Ristorante _ristoranteSelezionato;
        public Ristorante RistoranteSelezionato
        {
            get => _ristoranteSelezionato;
            set
            {
                _ristoranteSelezionato = value;
                OnPropertyChanged();
                EseguiAggiorna();
            }
        }

        private DateTime _dataInizio = DateTime.Today.AddMonths(-1);
        public DateTime DataInizio
        {
            get => _dataInizio;
            set
            {
                _dataInizio = value;
                OnPropertyChanged();
                EseguiAggiorna();
            }
        }

        private DateTime _dataFine = DateTime.Today;
        public DateTime DataFine
        {
            get => _dataFine;
            set
            {
                _dataFine = value;
                OnPropertyChanged();
                EseguiAggiorna();
            }
        }


        //proprieta per mostrare i risultati KPI
        //Key Performance Indicators

        private int _totalePrenotazioni;
        public int TotalePrenotazioni
        {
            get => _totalePrenotazioni;
            set { _totalePrenotazioni = value; OnPropertyChanged(); }
        }

        private int _totalePersone;
        public int TotalePersone
        {
            get => _totalePersone;
            set { _totalePersone = value; OnPropertyChanged(); }
        }

        private decimal _tassoOccupazione;
        public decimal TassoOccupazione
        {
            get => _tassoOccupazione;
            set { _tassoOccupazione = value; OnPropertyChanged(); }
        }

        public bool MostraTasso => RistoranteSelezionato != null;





        // proprieta per grafico
        private SeriesCollection _serieGrafico;
        public SeriesCollection SerieGrafico
        {
            get => _serieGrafico;
            set { _serieGrafico = value; OnPropertyChanged(); }
        }

        private List<string> _etichetteDateGrafico;
        public List<string> EtichetteDateGrafico
        {
            get => _etichetteDateGrafico;
            set { _etichetteDateGrafico = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }



        //comandi
        public ICommand AggiornCommand { get; }


        //costruttore
        public StatisticheViewModel(
            StatisticheManager statManager,
            RistoranteManager ristManager)
        {
            _statManager = statManager
                ?? throw new ArgumentNullException(nameof(statManager));
            _ristManager = ristManager
                ?? throw new ArgumentNullException(nameof(ristManager));

            AggiornCommand = new RelayCommand(EseguiAggiorna);

            CaricaRistoranti();
            //ora ho gia inizializzato il grafico
            _graficoInizializzato = true;

            EseguiAggiorna();
        }



        //logica di caricamento dati e aggiornamento grafico
        private void CaricaRistoranti()
        {
            try
            {
                var lista = _ristManager.GetAll();
                lista.Insert(0, new Ristorante
                {
                    IDRistorante = 0,
                    RagioneSociale = "Tutti i ristoranti"
                });
                ListaRistoranti = lista;
                RistoranteSelezionato = lista[0];
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

        private void EseguiAggiorna()
        {

            if (!_graficoInizializzato) return;

            try
            {
                MessaggioErrore = null;

                int? idRist = RistoranteSelezionato?.IDRistorante == 0
                    ? (int?)null
                    : RistoranteSelezionato?.IDRistorante;

                //KPI
                TotalePrenotazioni = _statManager
                    .GetTotalePrenotazioni(idRist, DataInizio, DataFine);
                TotalePersone = _statManager
                    .GetTotalePersone(idRist, DataInizio, DataFine);

                //Tasso occupazione, solo se un ristorante specifico
                if (idRist.HasValue)
                {
                    TassoOccupazione = _statManager
                        .GetTassoOccupazione(idRist.Value, DataInizio, DataFine);
                    OnPropertyChanged(nameof(MostraTasso));
                }
                else
                {
                    TassoOccupazione = 0;
                    OnPropertyChanged(nameof(MostraTasso));
                }

                //Dati grafico
                var datiGiorno = _statManager
                    .GetPerGiorno(idRist, DataInizio, DataFine);

                AggiornaGrafico(datiGiorno);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

        private void AggiornaGrafico(List<StatisticaGiornaliera> dati)
        {
            EtichetteDateGrafico = dati
                .Select(d => d.Data.ToString("dd/MM"))
                .ToList();

            SerieGrafico = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title  = "Prenotazioni",
                    Values = new ChartValues<int>(
                        dati.Select(d => d.NumPrenotazioni)),
                    Fill   = System.Windows.Media.Brushes.MediumSlateBlue
                },
                new LineSeries
                {
                    Title  = "Persone",
                    Values = new ChartValues<int>(
                        dati.Select(d => d.NumPersone)),
                    Stroke          = System.Windows.Media.Brushes.Teal,
                    Fill            = System.Windows.Media.Brushes.Transparent,
                    PointGeometrySize = 6
                }
            };
        }



    }
}
