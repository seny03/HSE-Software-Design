namespace ApiGateway.Models
{
    public class FileMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FileUploadResult
    {
        public string FileId { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
