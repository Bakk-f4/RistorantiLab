# RistorantiLab

A WPF desktop application built as a **learning lab** to practice the **MVVM** (Model-View-ViewModel) and **N-Tier** architectural patterns in C# with .NET Framework.

---

## Learning objectives

- Understand separation of concerns through N-Tier architecture
- Apply the MVVM pattern in a real WPF application
- Use `INotifyPropertyChanged` and WPF bindings to reactively update the UI
- Implement commands (`ICommand` / `RelayCommand`) to handle user actions without code-behind
- Access a SQL Server database via ADO.NET with parameterized queries (no SQL injection)
- Manage view navigation through a central `MainViewModel`

---

## N-Tier Architecture

The project is split into distinct layers, each with a single responsibility:

```
┌─────────────────────────────────────────┐
│   UI  (WPF – MVVM)                      │  Presentation Layer
│   Views + ViewModels + Helpers          │
├─────────────────────────────────────────┤
│   Manager                               │  Orchestration / Application Layer
├─────────────────────────────────────────┤
│   Engine                                │  Business Logic Layer
├─────────────────────────────────────────┤
│   IDal  (interfaces)                    │  Data Access Contract
│   DataDB (ADO.NET implementation)       │  Data Access Layer
├─────────────────────────────────────────┤
│   Entity                                │  Domain Models (POCO)
│   CoreFramework                         │  Cross-cutting (BusinessException, …)
└─────────────────────────────────────────┘
```

### Solution projects

| Project | Role |
|---|---|
| `Entity` | POCOs: `Ristorante`, `Utente`, `Prenotazione`, `StatisticaGiornaliera` |
| `CoreFramework` | Shared classes: `BusinessException` |
| `IDal` | DAL interfaces (`IRistoranteDAL`, `IUtenteDAL`, `IPrenotazioneDAL`) |
| `DataDB` | ADO.NET implementations that execute SQL Server queries |
| `Engine` | Business logic (`RistoranteEngine`, `UtenteEngine`, etc.) |
| `Manager` | Facade toward the UI: orchestrates Engine and validates high-level input |
| `UI` | WPF application with Views, ViewModels and Helpers |

---

## MVVM Pattern

The `UI` folder strictly follows the MVVM pattern:

```
UI/
├── Views/          ← Pure XAML, zero logic (minimal code-behind)
├── ViewModels/     ← Screen state and commands, no references to WPF controls
└── Helpers/
    ├── ViewModelBase.cs          ← Implements INotifyPropertyChanged
    ├── RelayCommand.cs           ← Implements ICommand with execute/canExecute delegates
    ├── NullToVisibilityConverter.cs
    └── SezioneToNavStyleConverter.cs
```

### Key elements

- **`ViewModelBase`** – abstract class that implements `INotifyPropertyChanged` via `[CallerMemberName]`, inherited by all ViewModels.
- **`RelayCommand`** – generic `ICommand` implementation that accepts `Action` and `Func<bool>` delegates, keeping code-behind out of the Views.
- **`MainViewModel`** – root ViewModel that manages navigation between sections (Restaurants, Bookings, Users, Statistics), the logged-in user's role (admin vs. regular), and logout.
- **Event-based navigation** – child ViewModels expose events (`RichiestaNuovo`, `RichiestaModifica`, `RichiestaTornaLista`) that `MainViewModel` intercepts to swap `CurrentView`, keeping ViewModels fully decoupled from each other.

---

## Application features

- **Login / Logout** with credential verification and session management
- **Restaurant management** – list with filters (city, type, price), create, edit, delete
- **Booking management** – list by restaurant and date, new booking, edit, cancel
- **User management** *(administrators only)* – list, create, edit users
- **Statistics** – daily statistics per restaurant

---

## Prerequisites

- Visual Studio 2022 (or later) with the **.NET desktop development** workload
- SQL Server (Express is fine) with a reachable instance
- .NET Framework 4.x (exact version specified in each `.csproj`)

---

## Configuration

The database connection string is read from `UI/.env`:

```
RISTORANTILAB_CONN=Server=<server>\SQLEXPRESS;Database=RistorantiLab;Integrated Security=True;
```

Update this file with your SQL Server instance details before running the application.

> **Note:** the `.env` file should not be committed with real credentials. Make sure it is listed in `.gitignore` or use Windows Integrated Security as shown in the example above.

---

## Getting started

1. Clone the repository
2. Open `RistorantiLab.slnx` in Visual Studio
3. Update `UI/.env` with your connection string
4. Create the database on SQL Server (DDL scripts not included – create tables `AnagraficaRistoranti`, `Utenti`, `Prenotazioni` based on the project entities)
5. Set `UI` as the startup project and press **F5**

---

## Code structure – quick reference

| Looking for | Where to look |
|---|---|
| Bindings and DataTemplates | `UI/Views/*.xaml` |
| Screen state and commands | `UI/ViewModels/*ViewModel.cs` |
| Navigation between screens | `UI/ViewModels/MainViewModel.cs` |
| Validation / business rules | `Engine/*Engine.cs` |
| SQL queries (ADO.NET) | `DataDB/*DAL.cs` |
| DAL contract | `IDal/I*DAL.cs` |
| Domain models | `Entity/*.cs` |
