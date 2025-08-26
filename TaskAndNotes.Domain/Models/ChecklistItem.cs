namespace TaskAndNotes.Domain.Models;

public sealed class ChecklistItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Text { get; set; }
    public bool IsCompleted { get; set; }
}


