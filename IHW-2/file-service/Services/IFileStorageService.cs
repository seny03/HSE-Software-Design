using FileService.Models;

namespace FileService.Services
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> SaveFileAsync(IFormFile file);
        Task<IEnumerable<FileMetadataDto>> GetAllFilesAsync();
        Task<FileDto?> GetFileByIdAsync(Guid id);
        Task<bool> DeleteFileAsync(Guid id);
    }
}
