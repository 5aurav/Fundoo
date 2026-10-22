
using Middleware.Services.Cache;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Commands
{
    public class ToggleArchiveCommandHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public ToggleArchiveCommandHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(ToggleArchiveCommand command)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                command.Model.NoteId,
                command.UserId);

            if (note == null || note.IsDeleted)
                return false;

            note.IsArchived = !note.IsArchived;

            await _notesRL.UpdateNoteAsync(note);

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:all");

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:note:{note.NoteId}");

            return true;
        }
    }
}
