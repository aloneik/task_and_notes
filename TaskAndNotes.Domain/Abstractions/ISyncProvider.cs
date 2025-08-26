using TaskAndNotes.Domain.Models;

namespace TaskAndNotes.Domain.Abstractions;

public interface ISyncProvider
{
    Task<IReadOnlyList<Note>> PullAsync(CancellationToken cancellationToken);
    Task PushAsync(IReadOnlyList<Note> notes, CancellationToken cancellationToken);
}


