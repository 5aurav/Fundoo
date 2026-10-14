using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Repository
{
    public class NotesRL : INotesRL
    {
        private readonly FundooContext _context;

        public NotesRL(FundooContext context)
        {
            _context = context;
        }

        public async Task<NoteEntity> AddNoteAsync(NoteEntity note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<List<NoteEntity>> ViewNotesAsync(int userId)
        {
            return await _context.Notes
                .AsNoTracking()
                .Where(n =>
                    n.UserId == userId &&
                    !n.IsDeleted &&
                    !n.IsArchived)
                .OrderByDescending(n => n.NoteId)
                .ToListAsync();
        }

        public async Task<NoteEntity?> GetNoteByIdAsync(
            int noteId, int userId)
        {
            return await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.NoteId == noteId &&
                    n.UserId == userId);
        }

        public async Task<NoteEntity> UpdateNoteAsync(NoteEntity note)
        {
            await _context.SaveChangesAsync();
            return note;
        }


        public async Task PermanentlyDeleteNoteAsync(NoteEntity note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }

    }
}