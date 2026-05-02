
using FilesUploading.Dto;
using FilesUploading.Models;
using FilesUploading.Repository;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FilesUploading.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _repository;
        private readonly FileStorageService _fileStorageService;

        // private static readonly string[] AllowedExtensions = { ".mp4", ".pdf", ".zip", ".rar" };
        private static readonly HashSet<string> AllowedExtensions = new() { ".mp4", ".pdf", ".zip", ".rar" };


        public FileService(IFileRepository repository, FileStorageService fileStorageService)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponseDto<string>> InitializeUpload(string fileName, long fileSize, int totalChunks)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetRandomFileName();
            string extension = GetFileExtensions(fileName);

            if (!AllowedExtensions.Contains(extension)) 
                return new ApiResponseDto<string> 
                { 
                    Message = "Extension Not Allowed", 
                    Success = false
                };

            var file = new Models.File
            {
                FileName = Guid.NewGuid() + extension,
                OriginalFileName = fileName,
                FileSize = fileSize,
                TotalChunks = totalChunks,
                Status = "uploading",
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateFileAsync(file);

            return result == null
                ? new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "Unable to initialize upload"
                }
                : new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Upload initialized successfully",
                    Data = result.Id.ToString()
                };
        }

        public async Task<string?> UploadChunk(int fileId, int chunkIndex, IFormFile chunk)
        {
            var file = await _repository.GetFileAsync(fileId);

            if (file == null) return null;

            // Check Duplicate
            if (await _repository.IsChunkExist(fileId, chunkIndex)) return "Chunk Already Uploaded";

            
            var uploadPath = _fileStorageService.GetUploadPath();
            var tempFilePath = Path.Combine(uploadPath, $"{file.FileName}.part");

            //Append Chunk
            using (var stream = new FileStream(tempFilePath, FileMode.Append)) await chunk.CopyToAsync(stream);

            // Save Chunk Info
            await _repository.AddChunkAsync(new FileChunk
            {
                FileId = fileId,
                ChunkIndex = chunkIndex,
                ChunkSize = chunk.Length,
                UploadedAt = DateTime.Now
            });

            // file.UploadedChunks = await _repository.GetUploadedChunkCount(fileId);

            if (string.IsNullOrEmpty(file.FileName)) return null;

            file.UploadedChunks += 1;
            if (file.UploadedChunks == file.TotalChunks)
            {
                var finalPath = Path.Combine(uploadPath, file.FileName);
                System.IO.File.Move(tempFilePath, finalPath);

                file.Status = "Completed";
            }

            // file.UploadedChunks += 1;

            _repository.UpdateFileAsync(file);
            await _repository.SaveChangesAsync();

            return file.Status;
        }
        // ---------------------------
        public async Task<string> UploadChunkV2(int fileId, int chunkIndex, IFormFile chunk)
        {
            var file = await _repository.GetFileAsync(fileId);
            if (file == null) return "File not found";

            var chunkFolder = GetPath(fileId);

            if (CheckDirectoryExist(chunkFolder)) Directory.CreateDirectory(chunkFolder);

            var chunkPath = Path.Combine(chunkFolder, $"{Path.GetFileNameWithoutExtension(file.FileName)}_{chunkIndex}");

            // prevent duplicate
            if (System.IO.File.Exists(chunkPath))
                return "Chunk already exists";

            // save chunk
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }

            // DB entry (optional but good)
            try
            {
                await _repository.AddChunkAsync(new FileChunk
                {
                    FileId = fileId,
                    ChunkIndex = chunkIndex,
                    ChunkSize = chunk.Length,
                    UploadedAt = DateTime.Now
                });
                file.UploadedChunks += 1;

                _repository.UpdateFileAsync(file);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                return "Chunk already uploaded (DB)";
            }

            return "Chunk uploaded";
        }

        public async Task<string?> MergeFileV2(int fileId)
        {
            bool isMissing = false;
            int missingIndex = -1;

            var file = await _repository.GetFileAsync(fileId);
            if (file == null) return "File not found";

            var chunkFolder = GetPath(fileId);

            if (CheckDirectoryExist(chunkFolder)) return "Chunks not found";

            if (string.IsNullOrEmpty(file.FileName)) return null;

            var finalPath = Path.Combine(chunkFolder, file.FileName);

            if (file.TotalChunks == file.UploadedChunks)
            {
                if (System.IO.File.Exists(finalPath))
                {
                    System.IO.File.Delete(finalPath);
                }

                using (var finalStream = new FileStream(finalPath, FileMode.Create))
                {
                    for (int i = 1; i <= file.TotalChunks; i++)
                    {
                        var chunkPath = Path.Combine(chunkFolder, $"{Path.GetFileNameWithoutExtension(file.FileName)}_{i}");

                        if (!System.IO.File.Exists(chunkPath))
                        {
                            isMissing = !isMissing;
                            missingIndex = i;
                            break;
                        }

                        using (var chunkStream = new FileStream(chunkPath, FileMode.Open))
                        {
                            await chunkStream.CopyToAsync(finalStream);
                        }
                        System.IO.File.Delete(chunkPath);
                    }
                }
            }

            if (isMissing)
            {
                if (System.IO.File.Exists(finalPath)) System.IO.File.Delete(finalPath);

                return $"Missing chunk {missingIndex}, Please Re-upload the Again";
            }

            // cleanup
            //Directory.Delete(chunkFolder, true);

            file.Status = "Completed";
            _repository.UpdateFileAsync(file);
            await _repository.SaveChangesAsync();

            return "File merged successfully";
        }

        public string GetPath(int fileId)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "NewUploads", fileId.ToString());
        }

        public bool CheckDirectoryExist(string path)
        {
            return !Directory.Exists(path);
        }
        public string GetFileExtensions(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension;
        }

    }
}
