
using Middleware.Services.Cache;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Notes.Queries
{
    public class GetAllNotesQueryHandler
    {
        private readonly INotesRL _notesRL;
        private readonly ICacheService _cacheService;

        public GetAllNotesQueryHandler(
            INotesRL notesRL,
            ICacheService cacheService)
        {
            _notesRL = notesRL;
            _cacheService = cacheService;
        }

        public async Task<List<NoteEntity>> Handle(GetAllNotesQuery query)
        {
            var cacheKey = $"notes:{query.UserId}:all";

            var cachedNotes =
                await _cacheService.GetAsync<List<NoteEntity>>(cacheKey);

            if (cachedNotes != null)
                return cachedNotes;

            var notes = await _notesRL.ViewNotesAsync(query.UserId);

            await _cacheService.SetAsync(
                cacheKey,
                notes,
                TimeSpan.FromMinutes(10));

            return notes;
        }
    }
}
