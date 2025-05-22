using AnalysisService.Models;

namespace AnalysisService.Services
{
    public interface ITextAnalysisService
    {
        Task<AnalysisResult> AnalyzeTextAsync(string fileId, string content);
        Task<AnalysisResult?> GetAnalysisAsync(Guid analysisId);
        Task<List<ComparisonResult>> CompareTextsAsync(List<string> fileIds, Dictionary<string, string> fileContents);
        double CalculateSimilarity(string text1, string text2);
        AnalysisStatistics CountStatistics(string text);
    }
}
