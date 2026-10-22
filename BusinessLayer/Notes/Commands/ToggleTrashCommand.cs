
using ModelLayer;

namespace BusinessLayer.Notes.Commands
{
    public record ToggleTrashCommand(NotesIdModel Model, int UserId);
}
