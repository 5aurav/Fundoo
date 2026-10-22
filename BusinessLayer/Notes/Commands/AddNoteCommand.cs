
using ModelLayer;

namespace BusinessLayer.Notes.Commands
{
    public record AddNoteCommand(NoteCreationModel Model, int UserId);
}