using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;

namespace FilesUploading.Dto
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        [JsonIgnore]
        public T? Data { get; set; }
    }
}
