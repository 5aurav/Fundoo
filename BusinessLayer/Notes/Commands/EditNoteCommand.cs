
using ModelLayer;

namespace BusinessLayer.Notes.Commands
{
    public record EditNoteCommand(EditNotesModel Model, int UserId);
}