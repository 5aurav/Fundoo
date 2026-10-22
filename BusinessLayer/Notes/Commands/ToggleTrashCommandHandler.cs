
using Middleware.Services.Cache;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Commands
{
    public class ToggleTrashCommandHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public ToggleTrashCommandHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(ToggleTrashCommand command)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                command.Model.NoteId,
                command.UserId);

            if (note == null)
                return false;

            note.IsDeleted = !note.IsDeleted;

            await _notesRL.UpdateNoteAsync(note);

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:all");

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:note:{note.NoteId}");

            return true;
        }
    }
}
