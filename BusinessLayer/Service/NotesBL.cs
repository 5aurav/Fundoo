
using BusinessLayer.Interface;
using BusinessLayer.Notes.Commands;
using BusinessLayer.Notes.Queries;
using ModelLayer;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service
{
    public class NotesBL : INotesBL
    {
        private readonly AddNoteCommandHandler _addNoteHandler;
        private readonly EditNoteCommandHandler _editNoteHandler;
        private readonly DeleteNoteCommandHandler _deleteNoteHandler;
        private readonly ToggleArchiveCommandHandler _toggleArchiveHandler;
        private readonly ToggleTrashCommandHandler _toggleTrashHandler;
        private readonly GetAllNotesQueryHandler _getAllNotesHandler;
        private readonly GetNoteByIdQueryHandler _getNoteByIdHandler;

        public NotesBL(
            AddNoteCommandHandler addNoteHandler,
            EditNoteCommandHandler editNoteHandler,
            DeleteNoteCommandHandler deleteNoteHandler,
            ToggleArchiveCommandHandler toggleArchiveHandler,
            ToggleTrashCommandHandler toggleTrashHandler,
            GetAllNotesQueryHandler getAllNotesHandler,
            GetNoteByIdQueryHandler getNoteByIdHandler)
        {
            _addNoteHandler = addNoteHandler;
            _editNoteHandler = editNoteHandler;
            _deleteNoteHandler = deleteNoteHandler;
            _toggleArchiveHandler = toggleArchiveHandler;
            _toggleTrashHandler = toggleTrashHandler;
            _getAllNotesHandler = getAllNotesHandler;
            _getNoteByIdHandler = getNoteByIdHandler;
        }

        public Task<NoteEntity> AddNoteAsync(
            NoteCreationModel model,
            int userId)
        {
            return _addNoteHandler.Handle(
                new AddNoteCommand(model, userId));
        }

        public Task<List<NoteEntity>> ViewNotesAsync(int userId)
        {
            return _getAllNotesHandler.Handle(
                new GetAllNotesQuery(userId));
        }

        public Task<NoteEntity?> ViewNoteByIdAsync(
            NotesIdModel model,
            int userId)
        {
            return _getNoteByIdHandler.Handle(
                new GetNoteByIdQuery(model, userId));
        }

        public Task<NoteEntity?> EditNoteAsync(
            EditNotesModel model,
            int userId)
        {
            return _editNoteHandler.Handle(
                new EditNoteCommand(model, userId));
        }

        public Task<bool> DeleteNoteAsync(
            NotesIdModel model,
            int userId)
        {
            return _deleteNoteHandler.Handle(
                new DeleteNoteCommand(model, userId));
        }

        public Task<bool> ToggleArchiveAsync(
            NotesIdModel model,
            int userId)
        {
            return _toggleArchiveHandler.Handle(
                new ToggleArchiveCommand(model, userId));
        }

        public Task<bool> ToggleTrashAsync(
            NotesIdModel model,
            int userId)
        {
            return _toggleTrashHandler.Handle(
                new ToggleTrashCommand(model, userId));
        }
    }
}
