
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity
{
    public class NoteEntity
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(10)]
        public string? Colour { get; set; }

        public bool IsArchived { get; set; } = false;

        [Required]
        public int UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; } = null!;
    }
}
