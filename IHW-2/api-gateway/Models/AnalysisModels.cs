namespace ApiGateway.Models
{
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

    public class UploadResponse
    {
        public string FileId { get; set; } = string.Empty;
        public string AnalysisId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
    }
}
