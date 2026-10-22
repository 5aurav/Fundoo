using ModelLayer;

namespace BusinessLayer.Notes.Queries
{
    public record GetNoteByIdQuery(NotesIdModel Model, int UserId);
}