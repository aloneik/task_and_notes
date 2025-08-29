using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskAndNotes.UI.Utils;
using TaskAndNotes.Domain.Models;
using TaskAndNotes.Application.Services;


namespace TaskAndNotes.UI.ViewModels;

public sealed class NoteViewModel : ObservableObject
{
    private readonly Note _model;
    private readonly NoteService? _noteService;
    private readonly Action<Guid>? _onDeleted;
    private readonly Action? _onSaved;
    private ChecklistItemViewModel? _selectedItem;
    private CancellationTokenSource? _saveDebounceCts;

    public NoteViewModel(Note model)
    {
        _model = model;
        Items = new ObservableCollection<ChecklistItemViewModel>(_model.Items.Select(i => new ChecklistItemViewModel(i)));
        InitializeCommands();
    }

    public NoteViewModel(Note model, NoteService noteService, Action<Guid>? onDeleted = null, Action? onSaved = null)
        : this(model)
    {
        _noteService = noteService;
        _onDeleted = onDeleted;
        _onSaved = onSaved;
        HookItemEvents();
        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    public Guid Id => _model.Id;

    public string Title
    {
        get => _model.Title;
        set
        {
            if (_model.Title != value)
            {
                _model.Title = value;
                OnPropertyChanged();
                ScheduleSave();
            }
        }
    }

    public string? Content
    {
        get => _model.Content;
        set
        {
            if (_model.Content != value)
            {
                _model.Content = value;
                OnPropertyChanged();
                ScheduleSave();
            }
        }
    }

    public ObservableCollection<ChecklistItemViewModel> Items { get; }

    public ChecklistItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                RemoveSelectedItemCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    public DateTimeOffset UpdatedAt => _model.UpdatedAt;

    public string Preview
    {
        get
        {
            var text = string.IsNullOrWhiteSpace(_model.Content) ? string.Empty : _model.Content!;
            if (text.Length <= 80)
            {
                return text;
            }
            return text.Substring(0, 80) + "…";
        }
    }

    public RelayCommand AddItemCommand { get; private set; } = null!;
    public RelayCommand RemoveSelectedItemCommand { get; private set; } = null!;
    public RelayCommand SaveCurrentNoteCommand { get; private set; } = null!;
    public RelayCommand DeleteCurrentNoteCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        AddItemCommand = new RelayCommand(AddItem);
        RemoveSelectedItemCommand = new RelayCommand(_ => RemoveSelectedItem(), _ => SelectedItem is not null);
        SaveCurrentNoteCommand = new RelayCommand(async _ => await SaveNowAsync());
        DeleteCurrentNoteCommand = new RelayCommand(async _ => await DeleteNowAsync());
    }

    private void AddItem()
    {
        var item = new ChecklistItem { Text = "", IsCompleted = false };
        var vm = new ChecklistItemViewModel(item);
        Items.Add(vm);
        SelectedItem = vm;
        ScheduleSave();
    }

    private void RemoveSelectedItem()
    {
        if (SelectedItem is null)
        {
            return;
        }
        var toRemove = SelectedItem;
        Items.Remove(toRemove);
        SelectedItem = Items.FirstOrDefault();
        ScheduleSave();
    }

    public Note ToModel()
    {
        _model.Items = Items.Select(vm => vm.ToModel()).ToList();
        _model.UpdatedAt = DateTimeOffset.UtcNow;
        return _model;
    }

    private async Task SaveNowAsync()
    {
        if (_noteService is null)
        {
            return;
        }
        var model = ToModel();
        var existing = await _noteService.GetByIdAsync(model.Id, CancellationToken.None);
        if (existing is null)
        {
            await _noteService.AddAsync(model, CancellationToken.None);
        }
        else
        {
            await _noteService.UpdateAsync(model, CancellationToken.None);
        }
        _onSaved?.Invoke();
    }

    private async Task DeleteNowAsync()
    {
        if (_noteService is null)
        {
            return;
        }
        await _noteService.DeleteAsync(Id, CancellationToken.None);
        _onDeleted?.Invoke(Id);
    }

    private void HookItemEvents()
    {
        foreach (var vm in Items)
        {
            vm.PropertyChanged += OnItemPropertyChanged;
        }
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var old in e.OldItems.OfType<ChecklistItemViewModel>())
            {
                old.PropertyChanged -= OnItemPropertyChanged;
            }
        }
        if (e.NewItems is not null)
        {
            foreach (var added in e.NewItems.OfType<ChecklistItemViewModel>())
            {
                added.PropertyChanged += OnItemPropertyChanged;
            }
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ScheduleSave();
    }

    private void ScheduleSave(int delayMs = 500)
    {
        if (_noteService is null)
        {
            return;
        }

        _saveDebounceCts?.Cancel();
        var cts = new CancellationTokenSource();
        _saveDebounceCts = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delayMs, cts.Token);
                var model = ToModel();
                var existing = await _noteService.GetByIdAsync(model.Id, CancellationToken.None);
                if (existing is null)
                {
                    await _noteService.AddAsync(model, CancellationToken.None);
                }
                else
                {
                    await _noteService.UpdateAsync(model, CancellationToken.None);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch
            {
            }
        });
    }
}


