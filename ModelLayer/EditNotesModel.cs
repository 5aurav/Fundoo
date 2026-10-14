using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class EditNotesModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int NoteId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(10)]
        public string? Colour { get; set; }
    }
}
