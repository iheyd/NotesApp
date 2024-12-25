using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using NotesApp;

public class NotesService : NotesApp.NotesService.NotesServiceBase
{
    private readonly NotesDbContext _dbContext;

    public NotesService(NotesDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    public override async Task<AddNoteResponse> AddNote(AddNoteRequest request, ServerCallContext context)
    {
        var note = request.Note;
        
        int newId = _dbContext.Notes.Any() ? _dbContext.Notes.Max(n => n.Id) + 1 : 1;
        
        var noteEntity = new NoteEntity(newId, note.Title, note.Content);

        _dbContext.Notes.Add(noteEntity);
        await _dbContext.SaveChangesAsync();

        return new AddNoteResponse { Id = noteEntity.Id };
    }

    public override async Task<Note> GetNote(GetNoteRequest request, ServerCallContext context)
    {
        var noteEntity = await _dbContext.Notes.FindAsync(request.Id);
        if (noteEntity != null)
        {
            return new Note
            {
                Id = noteEntity.Id,
                Title = noteEntity.Title,
                Content = noteEntity.Content
            };
        }
        context.Status = new Status(StatusCode.NotFound, "Заметка не найдена");
        return new Note();
    }

    public override async Task<GetAllNotesResponse> GetAllNotes(GetAllNotesRequest request, ServerCallContext context)
    {
        var notes = await _dbContext.Notes.ToListAsync();
        var response = new GetAllNotesResponse();
        response.Notes.AddRange(notes.Select(n => new Note
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content
        }));
        return response;
    }

    public override async Task<DeleteNoteResponse> DeleteNote(DeleteNoteRequest request, ServerCallContext context)
    {
        var noteEntity = await _dbContext.Notes.FindAsync(request.Id);
        if (noteEntity != null)
        {
            _dbContext.Notes.Remove(noteEntity);
            await _dbContext.SaveChangesAsync();
            return new DeleteNoteResponse();
        }
        context.Status = new Status(StatusCode.NotFound, "Заметка не найдена");
        return new DeleteNoteResponse();
    }
}