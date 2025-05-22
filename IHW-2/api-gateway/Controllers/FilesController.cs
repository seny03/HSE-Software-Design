using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileServiceClient fileServiceClient,
            ILogger<FilesController> logger)
        {
            _fileServiceClient = fileServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Get all files
        /// </summary>
        /// <returns>List of all files</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileMetadata>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                _logger.LogInformation("Getting all files");
                var files = await _fileServiceClient.GetAllFilesAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all files");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error retrieving files" });
            }
        }

        /// <summary>
        /// Get file by ID
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
                _logger.LogInformation("Getting file by ID: {FileId}", id);
                var file = await _fileServiceClient.GetFileByIdAsync(id);
                return Ok(file);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Error retrieving file with ID {id}" });
            }
        }
    }
}
