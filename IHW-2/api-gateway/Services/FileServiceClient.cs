using ApiGateway.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Services
{
    public class FileServiceClient : IFileServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FileServiceClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileServiceClient(IHttpClientFactory httpClientFactory, ILogger<FileServiceClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FileService");
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
                _logger.LogError(ex, "Error checking File Service health");
                return new ServiceHealthStatus { Status = "down" };
            }
        }

        public async Task<IEnumerable<FileMetadata>> GetAllFilesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/files");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<FileMetadata>>(content, _jsonOptions) ?? 
                       Enumerable.Empty<FileMetadata>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all files");
                throw;
            }
        }

        public async Task<FileDto> GetFileByIdAsync(string fileId)
        {
            try
            {
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
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "File not found: {FileId}", fileId);
                throw new KeyNotFoundException($"File with ID {fileId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileId}", fileId);
                throw;
            }
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(streamContent, "file", file.FileName);
                
                var response = await _httpClient.PostAsync("/files", content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FileUploadResult>(responseContent, _jsonOptions);
                
                if (result == null)
                {
                    throw new Exception("Failed to deserialize file upload result");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                throw;
            }
        }
    }
}
