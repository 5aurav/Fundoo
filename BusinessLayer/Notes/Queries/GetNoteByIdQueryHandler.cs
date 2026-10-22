
using Middleware.Services.Cache;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Queries
{
    public class GetNoteByIdQueryHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public GetNoteByIdQueryHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<NoteEntity?> Handle(GetNoteByIdQuery query)
        {
            var cacheKey =
                $"notes:{query.UserId}:note:{query.Model.NoteId}";

            var cachedNote =
                await _cacheService.GetAsync<NoteEntity>(cacheKey);

            if (cachedNote != null)
            {
                if (cachedNote.IsDeleted || cachedNote.IsArchived)
                    return null;

                return cachedNote;
            }

            var note = await _notesRL.GetNoteByIdAsync(
                query.Model.NoteId,
                query.UserId);

            if (note == null || note.IsDeleted || note.IsArchived)
                return null;

            await _cacheService.SetAsync(
                cacheKey,
                note,
                TimeSpan.FromMinutes(10));

            return note;
        }
    }
}
