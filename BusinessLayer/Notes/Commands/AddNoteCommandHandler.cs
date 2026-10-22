
using Middleware.Services.Cache;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Commands
{
    public class AddNoteCommandHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public AddNoteCommandHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<NoteEntity> Handle(AddNoteCommand command)
        {
            var model = command.Model;

            var note = new NoteEntity
            {
                Title = model.Title,
                Description = model.Description,
                Colour = model.Colour,
                UserId = command.UserId,
                IsArchived = false,
                IsDeleted = false
            };

            var createdNote = await _notesRL.AddNoteAsync(note);

            await InvalidateCacheAsync(
                command.UserId,
                createdNote.NoteId);

            return createdNote;
        }

        private async Task InvalidateCacheAsync(int userId, int noteId)
        {
            await _cacheService.RemoveAsync($"notes:{userId}:all");

            await _cacheService.RemoveAsync(
                $"notes:{userId}:note:{noteId}");
        }
    }
}
