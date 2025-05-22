using ApiGateway.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Services
{
    public class AnalysisServiceClient : IAnalysisServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AnalysisServiceClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AnalysisServiceClient(IHttpClientFactory httpClientFactory, ILogger<AnalysisServiceClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient("AnalysisService");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ServiceHealthStatus> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/health");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ServiceHealthStatus>(content, _jsonOptions) ?? 
                           new ServiceHealthStatus { Status = "up" };
                }
                
                return new ServiceHealthStatus { Status = "down" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Analysis Service health");
                return new ServiceHealthStatus { Status = "down" };
            }
        }

        public async Task<AnalysisResult> RequestAnalysisAsync(string fileId)
        {
            try
            {
                var request = new AnalysisRequest { FileId = fileId };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(request, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _httpClient.PostAsync("/analysis", jsonContent);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AnalysisResult>(content, _jsonOptions);
                
                if (result == null)
                {
                    throw new Exception($"Failed to deserialize analysis result for file {fileId}");
                }
                
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "File not found for analysis: {FileId}", fileId);
                throw new KeyNotFoundException($"File with ID {fileId} not found for analysis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting analysis for file: {FileId}", fileId);
                throw;
            }
        }

        public async Task<AnalysisResult> GetAnalysisResultAsync(string analysisId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/analysis/{analysisId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AnalysisResult>(content, _jsonOptions);
                
                if (result == null)
                {
                    throw new Exception($"Failed to deserialize analysis with ID {analysisId}");
                }
                
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Analysis not found: {AnalysisId}", analysisId);
                throw new KeyNotFoundException($"Analysis with ID {analysisId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis: {AnalysisId}", analysisId);
                throw;
            }
        }

        public async Task<List<ComparisonResult>> CompareFilesAsync(List<string> fileIds)
        {
            try
            {
                var request = new ComparisonRequest { FileIds = fileIds };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(request, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _httpClient.PostAsync("/compare", jsonContent);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<ComparisonResult>>(content, _jsonOptions);
                
                if (results == null)
                {
                    throw new Exception("Failed to deserialize comparison results");
                }
                
                return results;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "One or more files not found for comparison");
                throw new KeyNotFoundException("One or more files not found for comparison");
            }
            catch (Exception ex)
            {
                var fileIdsString = string.Join(", ", fileIds);
                _logger.LogError(ex, "Error comparing files: {FileIds}", fileIdsString);
                throw;
            }
        }

        public async Task<WordCloudResponse> GenerateWordCloudAsync(string fileId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/wordcloud/{fileId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WordCloudResponse>(content, _jsonOptions);
                
                if (result == null)
                {
                    throw new Exception($"Failed to deserialize word cloud result for file {fileId}");
                }
                
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "File not found for word cloud: {FileId}", fileId);
                throw new KeyNotFoundException($"File with ID {fileId} not found for word cloud");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating word cloud for file: {FileId}", fileId);
                throw;
            }
        }
    }
}
