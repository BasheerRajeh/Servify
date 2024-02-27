using System.ComponentModel.DataAnnotations;

namespace Servify.DTOs
{
    public class DocumentDto
    {
        [Required(ErrorMessage = "File name is required.")]
        [StringLength(255, ErrorMessage = "File name must be at most 255 characters long.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File path is required.")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "File size is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "Invalid file size.")]
        public long FileSize { get; set; }
    }

}
