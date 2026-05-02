using FilesUploading.Dto;
using FilesUploading.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FilesUploading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _services;

        public FileController(IFileService services)
        {
            _services = services;
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitUpload([FromForm] string fileName, [FromForm] long fileSize, [FromForm]  int totalChunks)
        {
            var file = await _services.InitializeUpload(fileName, fileSize, totalChunks);

            return file.Success ? Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = file.Message,
                Data = file.Data
            }) : BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                file
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadChunk([FromForm] int fileId, [FromForm] int chunkIndex, IFormFile chunk)
        {
            var result = await _services.UploadChunk(fileId, chunkIndex, chunk);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"{result}"
            });
        }

        [HttpPost("upload-chunk-v2")]
        public async Task<IActionResult> UploadChunkV2([FromForm] int fileId, [FromForm] int chunkIndex, IFormFile chunk)
        {
            var result = await _services.UploadChunkV2(fileId, chunkIndex, chunk);
            return Ok(new { status = result });
        }

        [HttpPost("merge-v2")]
        public async Task<IActionResult> MergeFile(int fileId)
        {
            var result = await _services.MergeFileV2(fileId);
            return Ok(new { status = result });
        }
    }
}
