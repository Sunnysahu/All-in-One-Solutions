using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilesUploading.Models
{
    public class FileChunk
    {
        [Key]
        public int Id { get; set; }

        public int FileId { get; set; }

        public int ChunkIndex { get; set; }

        public long ChunkSize { get; set; }

        public DateTime UploadedAt { get; set; }

        [ForeignKey("FileId")]
        public File File { get; set; } = null!;
    }
}
