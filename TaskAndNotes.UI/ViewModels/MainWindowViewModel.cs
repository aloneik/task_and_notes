using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskAndNotes.Application.Services;
using TaskAndNotes.Domain.Models;
using TaskAndNotes.UI.Utils;
using Avalonia;
using Avalonia.Styling;

namespace TaskAndNotes.UI.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly NoteService _noteService;

    private NoteViewModel? _selectedNote;
    private string _searchText = string.Empty;
    private int _selectedSortIndex = 0; // 0: Updated desc, 1: Title asc

    public ObservableCollection<NoteViewModel> Notes { get; } = new();
    public ObservableCollection<NoteViewModel> FilteredNotes { get; } = new();

    public NoteViewModel? SelectedNote
    {
        get => _selectedNote;
        set
        {
            if (SetProperty(ref _selectedNote, value))
            {
                SaveNoteCommand.RaiseCanExecuteChanged();
                DeleteNoteCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand NewNoteCommand { get; }
    public RelayCommand DeleteNoteCommand { get; }
    public RelayCommand SaveNoteCommand { get; }
    public RelayCommand RefreshNotesCommand { get; }

    public IReadOnlyList<string> SortOptions { get; } = new[] { "Updated (newest)", "Title (A→Z)" };

    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                global::Avalonia.Application.Current!.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilterAndSort();
            }
        }
    }

    public int SelectedSortIndex
    {
        get => _selectedSortIndex;
        set
        {
            if (SetProperty(ref _selectedSortIndex, value))
            {
                ApplyFilterAndSort();
            }
        }
    }

    public MainWindowViewModel(NoteService noteService)
    {
        _noteService = noteService;

        NewNoteCommand = new RelayCommand(NewNote);
        DeleteNoteCommand = new RelayCommand(_ => DeleteSelectedNote(), _ => SelectedNote is not null);
        SaveNoteCommand = new RelayCommand(_ => SaveSelectedNote(), _ => SelectedNote is not null);
        RefreshNotesCommand = new RelayCommand(async _ => await LoadAsync());

        _ = LoadAsync();
        // Initialize theme state from current application theme
        IsDarkTheme = global::Avalonia.Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        Notes.CollectionChanged += (_, __) => ApplyFilterAndSort();
    }

    private async Task LoadAsync(Guid? selectId = null)
    {
        var all = await _noteService.GetAllAsync(CancellationToken.None);
        Notes.Clear();
        foreach (var n in all)
        {
            Notes.Add(new NoteViewModel(n, _noteService));
        }
        ApplyFilterAndSort();

        if (selectId is Guid id)
        {
            var match = FilteredNotes.FirstOrDefault(n => n.Id == id) ?? Notes.FirstOrDefault(n => n.Id == id);
            if (match is not null)
            {
                SelectedNote = match;
                return;
            }
        }
        SelectedNote = FilteredNotes.FirstOrDefault() ?? Notes.FirstOrDefault();
    }

    private void NewNote()
    {
        var model = new Note { Title = "New note", Content = string.Empty };
        var vm = new NoteViewModel(model, _noteService);
        Notes.Insert(0, vm);
        SelectedNote = vm;
        ApplyFilterAndSort();
    }

    private async void DeleteSelectedNote()
    {
        if (SelectedNote is null)
        {
            return;
        }

        var toRemove = SelectedNote;
        Notes.Remove(toRemove);
        SelectedNote = Notes.FirstOrDefault();
        await _noteService.DeleteAsync(toRemove.Id, CancellationToken.None);
        await LoadAsync();
    }

    private async void SaveSelectedNote()
    {
        if (SelectedNote is null)
        {
            return;
        }

        var model = SelectedNote.ToModel();
        var existing = await _noteService.GetByIdAsync(model.Id, CancellationToken.None);
        if (existing is null)
        {
            await _noteService.AddAsync(model, CancellationToken.None);
        }
        else
        {
            await _noteService.UpdateAsync(model, CancellationToken.None);
        }
        await LoadAsync(model.Id);
    }

    private void ApplyFilterAndSort()
    {
        var query = Notes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(n => (!string.IsNullOrEmpty(n.Title) && n.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                                  || (!string.IsNullOrEmpty(n.Preview) && n.Preview.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        query = SelectedSortIndex switch
        {
            1 => query.OrderBy(n => n.Title),
            _ => query.OrderByDescending(n => n.UpdatedAt)
        };

        FilteredNotes.Clear();
        foreach (var n in query)
        {
            FilteredNotes.Add(n);
        }
    }
}


