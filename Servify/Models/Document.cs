using System.ComponentModel.DataAnnotations;

namespace Servify.Models
{
    public class Document: BaseEntity<int>
    {

        [Required(ErrorMessage = "File name is required.")]
        [StringLength(255, ErrorMessage = "File name must be at most 255 characters long.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File path is required.")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = "File size is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid file size.")]
        public long FileSize { get; set; }
    }

}
