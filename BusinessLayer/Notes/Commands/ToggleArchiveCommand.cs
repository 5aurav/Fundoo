
using ModelLayer;

namespace BusinessLayer.Notes.Commands
{
    public record ToggleArchiveCommand(NotesIdModel Model, int UserId);
}
