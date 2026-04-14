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

        public MainViewModel(RistoranteManager ristoranteManager)
        {
            _ristoranteManager = ristoranteManager
                ?? throw new ArgumentNullException(nameof(ristoranteManager));

            //Mostra la lista ristoranti all'avvio
            MostraLista();
        }

        public void MostraLista()
        {
            var vm = new RistoranteListViewModel(_ristoranteManager);

            //Sottoscrive gli eventi di navigazione dalla lista
            vm.RichiestaNuovo += OnRichiestanuovo;
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

        private void OnRichiestanuovo(object sender, EventArgs e)
            => MostraDettaglio(null);

        private void OnRichiestaModifica(object sender, Ristorante r)
            => MostraDettaglio(r);

        private void OnRichiestaTornaLista(object sender, EventArgs e)
            => MostraLista();


    }
}
