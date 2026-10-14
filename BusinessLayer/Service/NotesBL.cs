
using BusinessLayer.Interface;
using ModelLayer;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class NotesBL : INotesBL
    {
        private readonly INotesRL _notesRL;

        public NotesBL(INotesRL notesRL)
        {
            _notesRL = notesRL;
        }

        public async Task<NoteEntity> AddNoteAsync(
            NoteCreationModel model, int userId)
        {
            var note = new NoteEntity
            {
                Title = model.Title,
                Description = model.Description,
                Colour = model.Colour,
                UserId = userId,
                IsArchived = false,
                IsDeleted = false
            };

            return await _notesRL.AddNoteAsync(note);
        }

        public async Task<List<NoteEntity>> ViewNotesAsync(int userId)
        {
            return await _notesRL.ViewNotesAsync(userId);
        }


        public async Task<NoteEntity?> ViewNoteByIdAsync(
            NotesIdModel model, int userId)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId, userId);

            if (note == null || note.IsDeleted || note.IsArchived)
                return null;

            return note;
        }


        public async Task<NoteEntity?> EditNoteAsync(
            EditNotesModel model, int userId)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId, userId);

            if (note == null || note.IsDeleted)
                return null;

            note.Title = model.Title;
            note.Description = model.Description;
            note.Colour = model.Colour;

            return await _notesRL.UpdateNoteAsync(note);
        }


        public async Task<bool> DeleteNoteAsync(
            NotesIdModel model, int userId)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId, userId);

            if (note == null || !note.IsDeleted)
                return false;

            await _notesRL.PermanentlyDeleteNoteAsync(note);

            return true;
        }


        public async Task<bool> ToggleArchiveAsync(
            NotesIdModel model, int userId)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId, userId);

            if (note == null || note.IsDeleted)
                return false;

            note.IsArchived = !note.IsArchived;

            await _notesRL.UpdateNoteAsync(note);
            return true;
        }


        public async Task<bool> ToggleTrashAsync(
            NotesIdModel model, int userId)
        {
            var note = await _notesRL.GetNoteByIdAsync(
                model.NoteId, userId);

            if (note == null)
                return false;

            note.IsDeleted = !note.IsDeleted;

            await _notesRL.UpdateNoteAsync(note);

            return true;
        }

    }
}

