
using ModelLayer;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface INotesBL
    {
        Task<NoteEntity> AddNoteAsync(
            NoteCreationModel model, int userId);

        Task<List<NoteEntity>> ViewNotesAsync(int userId);

        Task<NoteEntity?> ViewNoteByIdAsync(
            NotesIdModel model, int userId);

        Task<NoteEntity?> EditNoteAsync(
            EditNotesModel model, int userId);

        Task<bool> DeleteNoteAsync(
            NotesIdModel model, int userId);

        Task<bool> ToggleArchiveAsync(
            NotesIdModel model, int userId);

        Task<bool> ToggleTrashAsync(
            NotesIdModel model, int userId);
    }
}
