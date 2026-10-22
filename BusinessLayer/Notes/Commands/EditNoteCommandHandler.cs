
using Middleware.Services.Cache;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Commands
{
    public class EditNoteCommandHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public EditNoteCommandHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<NoteEntity?> Handle(EditNoteCommand command)
        {
            var model = command.Model;

            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId,
                command.UserId);

            if (note == null || note.IsDeleted)
                return null;

            note.Title = model.Title;
            note.Description = model.Description;
            note.Colour = model.Colour;

            var updatedNote = await _notesRL.UpdateNoteAsync(note);

            await InvalidateCacheAsync(
                command.UserId,
                updatedNote.NoteId);

            return updatedNote;
        }

        private async Task InvalidateCacheAsync(int userId, int noteId)
        {
            await _cacheService.RemoveAsync($"notes:{userId}:all");

            await _cacheService.RemoveAsync(
                $"notes:{userId}:note:{noteId}");
        }
    }
}
