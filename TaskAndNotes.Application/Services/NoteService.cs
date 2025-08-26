using TaskAndNotes.Domain.Abstractions;
using TaskAndNotes.Domain.Models;

namespace TaskAndNotes.Application.Services;

public sealed class NoteService
{
    private readonly INoteRepository _repository;

    public NoteService(INoteRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct) => _repository.GetAllAsync(ct);
    public Task<Note?> GetByIdAsync(Guid id, CancellationToken ct) => _repository.GetByIdAsync(id, ct);
    public Task AddAsync(Note note, CancellationToken ct) => _repository.AddAsync(note, ct);
    public Task UpdateAsync(Note note, CancellationToken ct) => _repository.UpdateAsync(note, ct);
    public Task DeleteAsync(Guid id, CancellationToken ct) => _repository.DeleteAsync(id, ct);
}


