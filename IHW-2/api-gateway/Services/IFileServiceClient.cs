using ApiGateway.Models;

namespace ApiGateway.Services
{
    public interface IFileServiceClient
    {
        Task<ServiceHealthStatus> CheckHealthAsync();
        Task<FileUploadResult> UploadFileAsync(IFormFile file);
        Task<IEnumerable<FileMetadata>> GetAllFilesAsync();
        Task<FileDto> GetFileByIdAsync(string fileId);
    }
}
