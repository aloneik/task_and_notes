TaskAndNotes
============

Cross-platform desktop notes and checklists app built with Avalonia UI (.NET 9), using a clean layered architecture.

Features
- Create notes with title, content, and checklist items
- Mark checklist items as completed
- Auto-save with debounce; explicit Save/Delete actions
- Sidebar with search and sorting (Updated desc, Title A→Z)
- Pluggable persistence and sync abstractions (LiteDB local store, sync stub)

Tech
- UI: Avalonia 11, MVVM
- Application: orchestration services
- Domain: entities and repository/sync abstractions
- Infrastructure: LiteDB repository, NoOp sync provider

Run
- From the repository root:
```bash
dotnet restore
dotnet run -c Debug -p ./TaskAndNotes.UI/TaskAndNotes.UI.csproj
```
- Or Release:
```bash
dotnet run -c Release -p ./TaskAndNotes.UI/TaskAndNotes.UI.csproj
```
- On Windows PowerShell you can use backslashes:
```powershell
dotnet run -c Debug -p .\TaskAndNotes.UI\TaskAndNotes.UI.csproj
```

If you see a file lock warning while building, close the running app or run:
```powershell
taskkill /IM TaskAndNotes.UI.exe /F
```

Project structure
```
TaskAndNotes.sln
├─ TaskAndNotes.Domain/            # Domain models and abstractions
│  ├─ Models/Note.cs
│  ├─ Models/ChecklistItem.cs
│  └─ Abstractions/(INoteRepository, ISyncProvider)
├─ TaskAndNotes.Application/       # Application services
│  └─ Services/NoteService.cs
├─ TaskAndNotes.Infrastructure/    # Persistence & sync implementations
│  ├─ Persistence/LiteDbNoteRepository.cs
│  └─ Sync/NoOpSyncProvider.cs
└─ TaskAndNotes.UI/                # Avalonia UI (Views, ViewModels)
   ├─ Views/NoteView.axaml
   ├─ ViewModels/(MainWindowViewModel, NoteViewModel, ChecklistItemViewModel)
   ├─ Utils/(ObservableObject, RelayCommand)
   ├─ App.axaml, App.axaml.cs
   ├─ MainWindow.axaml, MainWindow.axaml.cs
   └─ Program.cs
```

Storage
- Local database: LiteDB file `notes.db` created next to the UI executable (working directory).
- You can replace LiteDB with another provider by implementing `INoteRepository` and wiring DI in `Program.cs`.

Sync (extensible)
- `ISyncProvider` abstraction for pull/push
- Default `NoOpSyncProvider` stub; you can add a real HTTP provider and call it from the application layer

Roadmap ideas
- Tags and filtering
- Rich text/markdown support
- Conflict resolution and remote sync
- Import/export

License
MIT


