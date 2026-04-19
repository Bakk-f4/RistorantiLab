using DataDB;
using dotenv.net;
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
using UI.Views;

namespace UI
{
    /// <summary>
    /// Classe principale dell'applicazione WPF. 
    /// Qui viene costruita la catena di dipendenze e viene avviata la prima finestra con il suo ViewModel.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DotEnv.Load();

            string connString = Environment.GetEnvironmentVariable(
                "RISTORANTILAB_CONN");

            if (string.IsNullOrWhiteSpace(connString))
                throw new InvalidOperationException(
                    "Variabile RISTORANTILAB_CONN non trovata nel file .env.");

            //ristorante
            var ristoranteDAL = new RistoranteDAL(connString);
            var ristoranteEngine = new RistoranteEngine(ristoranteDAL);
            var ristoranteManager = new RistoranteManager(ristoranteEngine);


            //utente
            var utenteDAL = new UtenteDAL(connString);
            var utenteEngine = new UtenteEngine(utenteDAL);
            var utenteManager = new UtenteManager(utenteEngine);

            //prenotazione
            var prenotazioneDAL = new PrenotazioneDAL(connString);
            var prenotazioneEngine = new PrenotazioneEngine(prenotazioneDAL, ristoranteDAL, utenteDAL);
            var prenotazioneManager = new PrenotazioneManager(prenotazioneEngine);

            //statistiche
            var statisticheEngine = new StatisticheEngine(prenotazioneDAL, ristoranteDAL);
            var statisticheManager = new StatisticheManager(statisticheEngine);

            //finestra di login
            var loginViewModel = new LoginViewModel(utenteManager);
            var loginWindow = new LoginWindow(loginViewModel);

            //showDialog blocca qui finche' la finestra non si chiude
            bool? loginRiuscito = loginWindow.ShowDialog();

            if (loginRiuscito != true)
            {
                Shutdown();
                return;
            }

            //Crea il ViewModel con i Manager iniettati
            //MainViewModel gestisce la navigazione
            var mainViewModel = new MainViewModel(
                ristoranteManager, utenteManager, prenotazioneManager,
                statisticheManager, loginViewModel.UtenteAutenticato);



            //Crea la finestra, assegna il DataContext e la mostra
            var mainWindow = new MainWindow();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Closed += (s, args) => Shutdown();

            mainWindow.Show();
        }

    }
}
