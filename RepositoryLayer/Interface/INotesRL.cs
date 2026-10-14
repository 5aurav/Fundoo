using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface INotesRL
    {
        Task<NoteEntity> AddNoteAsync(NoteEntity note);

        Task<List<NoteEntity>> ViewNotesAsync(int userId);

        Task<NoteEntity?> GetNoteByIdAsync(
            int noteId, int userId);

        Task<NoteEntity> UpdateNoteAsync(NoteEntity note);
        Task PermanentlyDeleteNoteAsync(NoteEntity note);
    }
}