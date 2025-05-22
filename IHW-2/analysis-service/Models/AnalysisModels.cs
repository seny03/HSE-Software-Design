using System.ComponentModel.DataAnnotations;

namespace AnalysisService.Models
{
    public class AnalysisEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid FileId { get; set; }
        
        [Required]
        public int ParagraphCount { get; set; }
        
        [Required]
        public int WordCount { get; set; }
        
        [Required]
        public int CharacterCount { get; set; }
        
        public string? WordCloudUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ComparisonEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid FileId1 { get; set; }
        
        [Required]
        public Guid FileId2 { get; set; }
        
        [Required]
        public double SimilarityScore { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AnalysisResult
    {
        public string AnalysisId { get; set; } = string.Empty;
        public string FileId { get; set; } = string.Empty;
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public string? WordCloudUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AnalysisRequest
    {
        public string FileId { get; set; } = string.Empty;
    }

    public class ComparisonRequest
    {
        public List<string> FileIds { get; set; } = new List<string>();
    }

    public class ComparisonResult
    {
        public string ComparisonId { get; set; } = string.Empty;
        public string FileId1 { get; set; } = string.Empty;
        public string FileId2 { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
        public bool IsPlagiarism { get; set; }
    }

    public class WordCloudResponse
    {
        public string WordCloudUrl { get; set; } = string.Empty;
    }

    public class FileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
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

    public class AnalysisStatistics
    {
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
    }

    public class WordFrequency
    {
        public string Word { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
