using AnalysisService.Models;

namespace AnalysisService.Services
{
    public interface IFileClientService
    {
        Task<FileDto> GetFileByIdAsync(string fileId);
        Task<Dictionary<string, string>> GetFileContentsAsync(List<string> fileIds);
    }
}
