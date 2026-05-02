using Microsoft.AspNetCore.Mvc;

namespace FilesUploading.Services
{
    public interface IFileService
    {
        Task<int?> InitializeUpload(string fileName, long fileSize, int totalChunks);
        Task<string?> UploadChunk(int fileId, int chunkIndex, IFormFile chunk);
        Task<string> UploadChunkV2(int fileId, int chunkIndex, IFormFile chunk);
        Task<string?> MergeFileV2(int fileId);
    }
}
