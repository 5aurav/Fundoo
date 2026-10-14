
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Security.Claims;

namespace Fundoo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INotesBL _notesBL;

        public NotesController(INotesBL notesBL)
        {
            _notesBL = notesBL;
        }

        private bool TryGetUserId(out int userId)
        {
            var userIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return int.TryParse(userIdValue, out userId);
        }

        [HttpPost("AddNote")]
        public async Task<IActionResult> AddNote(
            [FromBody] NoteCreationModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var note = await _notesBL.AddNoteAsync(model, userId);

            return Ok(new
            {
                message = "Note created successfully",
                data = note
            });
        }

        [HttpGet("ViewNote")]
        public async Task<IActionResult> ViewNote()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var notes = await _notesBL.ViewNotesAsync(userId);

            return Ok(notes);
        }

        [HttpPost("ViewNoteById")]
        public async Task<IActionResult> ViewNoteById(
            [FromBody] NotesIdModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var note = await _notesBL.ViewNoteByIdAsync(model, userId);

            if (note == null)
                return NotFound(new
                {
                    message = "Note not found"
                });

            return Ok(note);
        }

        [HttpPut("EditNote")]
        public async Task<IActionResult> EditNote(
            [FromBody] EditNotesModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var note = await _notesBL.EditNoteAsync(model, userId);

            if (note == null)
                return NotFound(new
                {
                    message = "Note not found"
                });

            return Ok(new
            {
                message = "Note updated successfully",
                data = note
            });
        }


        [HttpDelete("DeleteNote")]
        public async Task<IActionResult> DeleteNote(
            [FromBody] NotesIdModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var deleted = await _notesBL.DeleteNoteAsync(
                model, userId);

            if (!deleted)
                return BadRequest(new
                {
                    message = "Note must be in the trash before permanent deletion"
                });

            return Ok(new
            {
                message = "Note permanently deleted"
            });
        }


        [HttpPost("ArchUnarchived")]
        public async Task<IActionResult> ArchUnarchived(
            [FromBody] NotesIdModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var updated = await _notesBL.ToggleArchiveAsync(
                model, userId);

            if (!updated)
                return NotFound(new
                {
                    message = "Note not found"
                });

            return Ok(new
            {
                message = "Archive status updated"
            });
        }

        [HttpPost("TrashUntrash")]
        public async Task<IActionResult> TrashUntrash(
            [FromBody] NotesIdModel model)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var updated = await _notesBL.ToggleTrashAsync(
                model, userId);

            if (!updated)
                return NotFound(new
                {
                    message = "Note not found"
                });

            return Ok(new
            {
                message = "Trash status updated"
            });
        }
    }
}
