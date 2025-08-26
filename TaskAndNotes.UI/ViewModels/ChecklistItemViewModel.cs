using System;
using TaskAndNotes.Domain.Models;
using TaskAndNotes.UI.Utils;

namespace TaskAndNotes.UI.ViewModels;

public sealed class ChecklistItemViewModel : ObservableObject
{
    private readonly ChecklistItem _model;

    public ChecklistItemViewModel(ChecklistItem model)
    {
        _model = model;
    }

    public Guid Id => _model.Id;

    public string Text
    {
        get => _model.Text;
        set
        {
            if (_model.Text != value)
            {
                _model.Text = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsCompleted
    {
        get => _model.IsCompleted;
        set
        {
            if (_model.IsCompleted != value)
            {
                _model.IsCompleted = value;
                OnPropertyChanged();
            }
        }
    }

    public ChecklistItem ToModel() => _model;
}


