using System.ComponentModel.DataAnnotations;

namespace FileService.Models
{
    public class FileEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Filename { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1024)]
        public string FilePath { get; set; } = string.Empty;
        
        [Required]
        public long Size { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FileMetadataDto
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

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
    }
    
    public class ServiceHealthStatus
    {
        public string Status { get; set; } = "up";
    }
}
