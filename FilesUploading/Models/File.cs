using System.ComponentModel.DataAnnotations;

namespace FilesUploading.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string? FileName { get; set; }

        [Required]
        public string? OriginalFileName { get; set; }

        public long FileSize { get; set; }

        public int TotalChunks { get; set; }

        public int UploadedChunks { get; set; }

        [Required]
        public string Status { get; set; } = "Uploading";

        public DateTime CreatedAt { get; set; }

        public ICollection<FileChunk> Chunks { get; set; } = new List<FileChunk>();
}
}
