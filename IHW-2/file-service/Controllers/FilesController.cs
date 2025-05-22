using FileService.Models;
using FileService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileStorageService fileStorageService,
            ILogger<FilesController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Upload a new file
        /// </summary>
        /// <returns>File metadata</returns>
        [HttpPost]
        [ProducesResponseType(typeof(FileUploadResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ErrorResponse { Error = "No file uploaded" });
                }

                if (file.ContentType != "text/plain")
                {
                    return BadRequest(new ErrorResponse { Error = "Only .txt files are allowed" });
                }

                _logger.LogInformation("Uploading file: {FileName}", file.FileName);
                var result = await _fileStorageService.SaveFileAsync(file);
                
                return CreatedAtAction(nameof(GetFileById), new { id = result.FileId }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid file upload: {FileName}", file?.FileName);
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error uploading file" });
            }
        }

        /// <summary>
        /// Get all files
        /// </summary>
        /// <returns>List of all files</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                _logger.LogInformation("Getting all files");
                var files = await _fileStorageService.GetAllFilesAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving files");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error retrieving files" });
            }
        }

        /// <summary>
        /// Get a file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>File content and metadata</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFileById(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var fileId))
                {
                    return BadRequest(new ErrorResponse { Error = "Invalid file ID format" });
                }

                _logger.LogInformation("Getting file by ID: {FileId}", id);
                var file = await _fileStorageService.GetFileByIdAsync(fileId);
                
                if (file == null)
                {
                    return NotFound(new ErrorResponse { Error = $"File with ID {id} not found" });
                }
                
                return Ok(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Error retrieving file with ID {id}" });
            }
        }

        /// <summary>
        /// Delete a file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFile(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var fileId))
                {
                    return BadRequest(new ErrorResponse { Error = "Invalid file ID format" });
                }

                _logger.LogInformation("Deleting file by ID: {FileId}", id);
                var deleted = await _fileStorageService.DeleteFileAsync(fileId);
                
                if (!deleted)
                {
                    return NotFound(new ErrorResponse { Error = $"File with ID {id} not found" });
                }
                
                return Ok(new { message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Error deleting file with ID {id}" });
            }
        }
    }
}
