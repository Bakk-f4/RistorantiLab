# RistorantiLab

Un'applicazione desktop WPF sviluppata come **laboratorio didattico** per apprendere i pattern architetturali **MVVM** (Model-View-ViewModel) e **N-Tier** in C# con .NET Framework.

---

## Obiettivi didattici

- Comprendere la separazione delle responsabilità tramite l'architettura N-Tier
- Applicare il pattern MVVM in un'applicazione WPF reale
- Utilizzare `INotifyPropertyChanged` e i binding WPF per aggiornare la UI in modo reattivo
- Implementare comandi (`ICommand` / `RelayCommand`) per gestire le azioni utente senza code-behind
- Accedere a un database SQL Server tramite ADO.NET con parametri sicuri (niente SQL injection)
- Gestire la navigazione tra viste tramite un `MainViewModel` centrale

---

## Architettura N-Tier

Il progetto è suddiviso in layer distinti, ciascuno con una responsabilità specifica:

```
┌─────────────────────────────────────────┐
│   UI  (WPF – MVVM)                      │  Presentazione
│   Views + ViewModels + Helpers          │
├─────────────────────────────────────────┤
│   Manager                               │  Orchestrazione / Application Layer
├─────────────────────────────────────────┤
│   Engine                                │  Business Logic Layer
├─────────────────────────────────────────┤
│   IDal  (interfacce)                    │  Contratto Data Access
│   DataDB (implementazione ADO.NET)      │  Data Access Layer
├─────────────────────────────────────────┤
│   Entity                                │  Modelli di dominio (POCO)
│   CoreFramework                         │  Cross-cutting (BusinessException, …)
└─────────────────────────────────────────┘
```

### Progetti della soluzione

| Progetto | Ruolo |
|---|---|
| `Entity` | POCO: `Ristorante`, `Utente`, `Prenotazione`, `StatisticaGiornaliera` |
| `CoreFramework` | Classi condivise: `BusinessException` |
| `IDal` | Interfacce DAL (`IRistoranteDAL`, `IUtenteDAL`, `IPrenotazioneDAL`) |
| `DataDB` | Implementazioni ADO.NET che eseguono query SQL Server |
| `Engine` | Logica di business (`RistoranteEngine`, `UtenteEngine`, ecc.) |
| `Manager` | Facciata verso la UI: coordina Engine e valida input di alto livello |
| `UI` | Applicazione WPF con Views, ViewModels e Helpers |

---

## Pattern MVVM

La cartella `UI` segue rigorosamente il pattern MVVM:

```
UI/
├── Views/          ← XAML puri, zero logica (code-behind minimale)
├── ViewModels/     ← Stato e comandi della schermata, nessun riferimento a controlli WPF
└── Helpers/
    ├── ViewModelBase.cs          ← Implementa INotifyPropertyChanged
    ├── RelayCommand.cs           ← Implementa ICommand con delegate execute/canExecute
    ├── NullToVisibilityConverter.cs
    └── SezioneToNavStyleConverter.cs
```

### Elementi chiave

- **`ViewModelBase`** – classe astratta che implementa `INotifyPropertyChanged` tramite `[CallerMemberName]`, ereditata da tutti i ViewModel.
- **`RelayCommand`** – implementazione generica di `ICommand` che accetta delegate `Action` e `Func<bool>`, evitando il code-behind nelle Views.
- **`MainViewModel`** – ViewModel principale che gestisce navigazione tra sezioni (Ristoranti, Prenotazioni, Utenti, Statistiche), ruolo dell'utente loggato (admin vs. normale) e logout.
- **Navigazione via eventi** – i ViewModel figli espongono eventi (`RichiestaNuovo`, `RichiestaModifica`, `RichiestaTornaLista`) che `MainViewModel` intercetta per cambiare `CurrentView`, mantenendo i ViewModel disaccoppiati.

---

## Funzionalità dell'applicazione

- **Login / Logout** con verifica credenziali e gestione sessione
- **Gestione Ristoranti** – lista con filtri (città, tipologia, prezzo), inserimento, modifica, eliminazione
- **Gestione Prenotazioni** – lista per ristorante e data, nuova prenotazione, modifica, cancellazione
- **Gestione Utenti** *(solo amministratori)* – lista, inserimento, modifica utenti
- **Statistiche** – statistiche giornaliere per ristorante

---

## Prerequisiti

- Visual Studio 2022 (o successivo) con workload **.NET desktop development**
- SQL Server (anche Express) con un'istanza raggiungibile
- .NET Framework 4.x (versione indicata in ogni `.csproj`)

---

## Configurazione

La stringa di connessione al database è letta dal file `UI/.env`:

```
RISTORANTILAB_CONN=Server=<server>\SQLEXPRESS;Database=RistorantiLab;Integrated Security=True;
```

Modifica questo file con i dati della tua istanza SQL Server prima di avviare l'applicazione.

> **Nota:** il file `.env` non deve essere committato con credenziali reali. Assicurati che sia incluso nel `.gitignore` oppure usa Windows Integrated Security come nell'esempio.

---

## Avvio

1. Clona il repository
2. Apri `RistorantiLab.slnx` in Visual Studio
3. Aggiorna `UI/.env` con la tua stringa di connessione
4. Crea il database su SQL Server (script DDL non inclusi – crea le tabelle `AnagraficaRistoranti`, `Utenti`, `Prenotazioni` in base alle entità del progetto)
5. Imposta `UI` come progetto di avvio e premi **F5**

---

## Struttura del codice – riferimenti rapidi

| Cosa cercare | Dove guardare |
|---|---|
| Binding e DataTemplate | `UI/Views/*.xaml` |
| Stato e comandi di una schermata | `UI/ViewModels/*ViewModel.cs` |
| Navigazione tra schermate | `UI/ViewModels/MainViewModel.cs` |
| Validazione / regole di business | `Engine/*Engine.cs` |
| Query SQL (ADO.NET) | `DataDB/*DAL.cs` |
| Contratto del DAL | `IDal/I*DAL.cs` |
| Modelli di dominio | `Entity/*.cs` |
