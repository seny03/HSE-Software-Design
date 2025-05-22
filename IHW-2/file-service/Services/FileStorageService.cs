using FileService.Data;
using FileService.Models;
using Microsoft.EntityFrameworkCore;

namespace FileService.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly FileDbContext _dbContext;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _uploadsDirectory;

        public FileStorageService(
            FileDbContext dbContext,
            ILogger<FileStorageService> logger,
            IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            
            // Set uploads directory path
            _uploadsDirectory = Path.Combine(environment.ContentRootPath, "uploads");
            
            // Ensure directory exists
            if (!Directory.Exists(_uploadsDirectory))
            {
                Directory.CreateDirectory(_uploadsDirectory);
            }
        }

        public async Task<FileUploadResult> SaveFileAsync(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Length == 0)
            {
                throw new ArgumentException("File is empty", nameof(file));
            }

            if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only text files are allowed", nameof(file));
            }

            try
            {
                // Generate a unique identifier for the file
                var fileId = Guid.NewGuid();
                var filePath = Path.Combine(_uploadsDirectory, fileId.ToString());
                
                // Read file content
                string content;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    content = await reader.ReadToEndAsync();
                }
                
                // Save file to disk
                await System.IO.File.WriteAllTextAsync(filePath, content);
                
                // Create file entity
                var fileEntity = new FileEntity
                {
                    Id = fileId,
                    Filename = file.FileName,
                    FilePath = filePath,
                    Size = file.Length,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Save to database
                _dbContext.Files.Add(fileEntity);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("File saved: {Filename}, ID: {FileId}", file.FileName, fileId);
                
                // Return result
                return new FileUploadResult
                {
                    FileId = fileId.ToString(),
                    Filename = fileEntity.Filename,
                    Size = fileEntity.Size,
                    CreatedAt = fileEntity.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {Filename}", file.FileName);
                throw;
            }
        }

        public async Task<IEnumerable<FileMetadataDto>> GetAllFilesAsync()
        {
            try
            {
                var files = await _dbContext.Files
                    .OrderByDescending(f => f.CreatedAt)
                    .Select(f => new FileMetadataDto
                    {
                        Id = f.Id.ToString(),
                        Filename = f.Filename,
                        Size = f.Size,
                        CreatedAt = f.CreatedAt
                    })
                    .ToListAsync();
                
                _logger.LogInformation("Retrieved {Count} files", files.Count);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving files");
                throw;
            }
        }

        public async Task<FileDto?> GetFileByIdAsync(Guid id)
        {
            try
            {
                var file = await _dbContext.Files.FindAsync(id);
                
                if (file == null)
                {
                    _logger.LogWarning("File not found: {FileId}", id);
                    return null;
                }
                
                _logger.LogInformation("File retrieved: {Filename}, ID: {FileId}", file.Filename, id);
                
                return new FileDto
                {
                    Id = file.Id.ToString(),
                    Filename = file.Filename,
                    Content = file.Content,
                    Size = file.Size,
                    CreatedAt = file.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            try
            {
                var file = await _dbContext.Files.FindAsync(id);
                
                if (file == null)
                {
                    _logger.LogWarning("File not found for deletion: {FileId}", id);
                    return false;
                }
                
                // Delete file from disk if it exists
                if (System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }
                
                // Remove from database
                _dbContext.Files.Remove(file);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("File deleted: {Filename}, ID: {FileId}", file.Filename, id);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileId}", id);
                throw;
            }
        }
    }
}
