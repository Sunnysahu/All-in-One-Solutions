namespace FilesUploading.Services
{
    public class FileStorageService
    {
        private readonly string _uploadPath;

        public FileStorageService()
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public string GetUploadPath()
        {
            return _uploadPath;
        }
    }
}
