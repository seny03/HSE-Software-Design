using ApiGateway.Models;

namespace ApiGateway.Services
{
    public interface IAnalysisServiceClient
    {
        Task<ServiceHealthStatus> CheckHealthAsync();
        Task<AnalysisResult> RequestAnalysisAsync(string fileId);
        Task<AnalysisResult> GetAnalysisResultAsync(string analysisId);
        Task<List<ComparisonResult>> CompareFilesAsync(List<string> fileIds);
        Task<WordCloudResponse> GenerateWordCloudAsync(string fileId);
    }
}
