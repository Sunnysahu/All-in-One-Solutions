using FilesUploading.Models;

namespace FilesUploading.Repository
{
    public interface IFileRepository
    {
        Task<Models.File?> GetFileAsync(int fileId);
        Task<Models.File?> CreateFileAsync(Models.File file);
        void UpdateFileAsync(Models.File file);

        Task<bool> IsChunkExist(int fileId, int chunkIndex);
        Task AddChunkAsync(FileChunk chunk);
        Task<int> GetUploadedChunkCount(int fileId);
        Task SaveChangesAsync();
    }
}
