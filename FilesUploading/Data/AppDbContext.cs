using FilesUploading.Models;
using Microsoft.EntityFrameworkCore;

namespace FilesUploading.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Models.File> Files => Set<Models.File>();
        public DbSet<FileChunk> FileChunks => Set<FileChunk>();
    }
}
