
using ModelLayer;

namespace BusinessLayer.Notes.Commands
{
    public record DeleteNoteCommand(NotesIdModel Model, int UserId);
}
