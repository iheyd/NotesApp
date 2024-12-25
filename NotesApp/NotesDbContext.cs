using Microsoft.EntityFrameworkCore;
using NotesApp;

public class NotesDbContext : DbContext
{
    public DbSet<NoteEntity> Notes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=notes.db");
    }
}