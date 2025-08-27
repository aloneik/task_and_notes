using LiteDB;
using TaskAndNotes.Domain.Abstractions;
using TaskAndNotes.Domain.Models;

namespace TaskAndNotes.Infrastructure.Persistence;

public sealed class LiteDbNoteRepository : INoteRepository, IDisposable
{
    private readonly LiteDatabase _db;
    private readonly ILiteCollection<Note> _notes;

    public LiteDbNoteRepository(string databasePath)
    {
        _db = new LiteDatabase(databasePath);
        _notes = _db.GetCollection<Note>("notes");
        _notes.EnsureIndex(n => n.Id, true);
    }

    public Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken cancellationToken)
    {
        var list = _notes.FindAll().ToList();
        return Task.FromResult((IReadOnlyList<Note>)list);
    }

    public Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var note = _notes.FindById(id);
        return Task.FromResult((Note?)note);
    }

    public Task AddAsync(Note note, CancellationToken cancellationToken)
    {
        note.UpdatedAt = DateTimeOffset.UtcNow;
        _notes.Insert(note);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Note note, CancellationToken cancellationToken)
    {
        note.UpdatedAt = DateTimeOffset.UtcNow;
        _notes.Update(note);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _notes.Delete(id);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}


