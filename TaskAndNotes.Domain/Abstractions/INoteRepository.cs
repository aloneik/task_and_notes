using TaskAndNotes.Domain.Models;

namespace TaskAndNotes.Domain.Abstractions;

public interface INoteRepository
{
    Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken cancellationToken);
    Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Note note, CancellationToken cancellationToken);
    Task UpdateAsync(Note note, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}


