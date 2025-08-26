using TaskAndNotes.Domain.Abstractions;
using TaskAndNotes.Domain.Models;

namespace TaskAndNotes.Infrastructure.Sync;

public sealed class NoOpSyncProvider : ISyncProvider
{
    public Task<IReadOnlyList<Note>> PullAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IReadOnlyList<Note>)Array.Empty<Note>());
    }

    public Task PushAsync(IReadOnlyList<Note> notes, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


