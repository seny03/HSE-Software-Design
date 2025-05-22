using AnalysisService.Models;

namespace AnalysisService.Services
{
    public interface IWordCloudService
    {
        string GenerateWordCloudUrl(string text);
        Task<string> GetOrGenerateWordCloudUrlAsync(string fileId, string content);
    }
}
