using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IAnalysisServiceClient _analysisServiceClient;
        private readonly ILogger<UploadController> _logger;

        public UploadController(
            IFileServiceClient fileServiceClient,
            IAnalysisServiceClient analysisServiceClient,
            ILogger<UploadController> logger)
        {
            _fileServiceClient = fileServiceClient;
            _analysisServiceClient = analysisServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Upload a text file for analysis
        /// </summary>
        /// <returns>File ID and analysis ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ErrorResponse { Error = "No file uploaded" });
            }

            if (file.ContentType != "text/plain")
            {
                return BadRequest(new ErrorResponse { Error = "Only .txt files are allowed" });
            }

            try
            {
                _logger.LogInformation("Uploading file: {FileName}", file.FileName);
                
                // Send to File Service
                var fileResult = await _fileServiceClient.UploadFileAsync(file);
                
                _logger.LogInformation("File uploaded successfully: {FileId}", fileResult.FileId);
                
                try
                {
                    // Request analysis from Analysis Service
                    var analysisResult = await _analysisServiceClient.RequestAnalysisAsync(fileResult.FileId);
                    
                    _logger.LogInformation("Analysis requested successfully: {AnalysisId}", analysisResult.AnalysisId);
                    
                    return Ok(new UploadResponse
                    {
                        FileId = fileResult.FileId,
                        AnalysisId = analysisResult.AnalysisId,
                        Message = "File uploaded and analysis initiated"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error requesting analysis for file: {FileId}", fileResult.FileId);
                    
                    // Return partial success if the file was saved but analysis failed
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                    {
                        FileId = fileResult.FileId,
                        Error = "Analysis service unavailable, file saved but analysis failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                
                if (ex.Message.Contains("File service"))
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                        new ErrorResponse { Error = "File service unavailable" });
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error uploading file" });
            }
        }
    }
}
