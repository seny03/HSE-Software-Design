using AnalysisService.Models;
using System.Text.Json;

namespace AnalysisService.Services
{
    public class FileClientService : IFileClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FileClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileClientService(IHttpClientFactory httpClientFactory, ILogger<FileClientService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FileService");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<FileDto> GetFileByIdAsync(string fileId)
        {
            try
            {
                _logger.LogInformation("Getting file from File Service: {FileId}", fileId);
                
                var response = await _httpClient.GetAsync($"/files/{fileId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var file = JsonSerializer.Deserialize<FileDto>(content, _jsonOptions);
                
                if (file == null)
                {
                    throw new Exception($"Failed to deserialize file with ID {fileId}");
                }
                
                return file;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting file from File Service: {FileId}", fileId);
                throw new KeyNotFoundException($"File with ID {fileId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file from File Service: {FileId}", fileId);
                throw;
            }
        }

        public async Task<Dictionary<string, string>> GetFileContentsAsync(List<string> fileIds)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var fileId in fileIds.Distinct())
            {
                try
                {
                    var file = await GetFileByIdAsync(fileId);
                    result.Add(fileId, file.Content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting file content: {FileId}", fileId);
                    throw;
                }
            }
            
            return result;
        }
    }
}
