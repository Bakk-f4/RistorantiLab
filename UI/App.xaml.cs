using DataDB;
using Engine;
using Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.ViewModels;

namespace UI
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //leggi la connection string da App.config
            string connString = ConfigurationManager
                .ConnectionStrings["RistorantiLab"]
                .ConnectionString;

            //ristorante
            var ristoranteDAL = new RistoranteDAL(connString);
            var ristoranteEngine = new RistoranteEngine(ristoranteDAL);
            var ristoranteManager = new RistoranteManager(ristoranteEngine);

            //utente
            var utenteDAL = new UtenteDAL(connString);
            var utenteEngine = new UtenteEngine(utenteDAL);
            var utenteManager = new UtenteManager(utenteEngine);


            //crea il ViewModel con il Manager iniettato
            var mainViewModel = new MainViewModel(ristoranteManager, utenteManager);


            //crea la finestra, assegna il DataContext e la mostra
            var mainWindow = new MainWindow();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }
}
