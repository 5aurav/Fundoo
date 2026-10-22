
using Middleware.Services.Cache;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Commands
{
    public class DeleteNoteCommandHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public DeleteNoteCommandHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(DeleteNoteCommand command)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                command.Model.NoteId,
                command.UserId);

            if (note == null || !note.IsDeleted)
                return false;

            await _notesRL.PermanentlyDeleteNoteAsync(note);

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:all");

            await _cacheService.RemoveAsync(
                $"notes:{command.UserId}:note:{note.NoteId}");

            return true;
        }
    }
}
