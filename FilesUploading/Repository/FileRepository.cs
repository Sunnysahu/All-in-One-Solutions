using FilesUploading.Data;
using FilesUploading.Models;
using Microsoft.EntityFrameworkCore;

namespace FilesUploading.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddChunkAsync(FileChunk chunk)
        {
            await _context.FileChunks.AddAsync(chunk);
            // await _context.SaveChangesAsync();

        }

        public async Task<Models.File> CreateFileAsync(Models.File file)
        {
            await _context.Files.AddAsync(file);
            int rows = await _context.SaveChangesAsync();

            return file;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Models.File?> GetFileAsync(int fileId)
        {
            return await _context.Files.Include(f => f.Chunks).FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<int> GetUploadedChunkCount(int fileId)
        {
            return await _context.FileChunks.CountAsync(c => c.FileId == fileId);
        }

        public void UpdateFileAsync(Models.File file)
        {

            // Better Approach (Load + modify)
            /*
                var file = await _context.FileUploads.FindAsync(id);

                if (file == null) return;

                file.Name = newName;

                await _context.SaveChangesAsync();
             */

            // Partial update -- Updates ONLY Name
            /*
                _context.FileUploads.Attach(file);
                _context.Entry(file).Property(x => x.Name).IsModified = true;

                await _context.SaveChangesAsync();
             */

            // Bulk update (EF Core 7+)
            /*
                await _context.FileUploads.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.Name, "new name"));
             */

            _context.Files.Update(file);
            // await _context.SaveChangesAsync();
        }

        public async Task<bool> IsChunkExist(int fileId, int chunkIndex)
        {
            return await _context.FileChunks.AnyAsync(c => c.FileId == fileId && c.ChunkIndex == chunkIndex);
        }

    }
}
