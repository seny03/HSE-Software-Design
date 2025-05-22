using AnalysisService.Data;
using AnalysisService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AnalysisService.Services
{
    public class WordCloudService : IWordCloudService
    {
        private readonly AnalysisDbContext _dbContext;
        private readonly ILogger<WordCloudService> _logger;
        
        public WordCloudService(AnalysisDbContext dbContext, ILogger<WordCloudService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public string GenerateWordCloudUrl(string text)
        {
            try
            {
                var words = ExtractWords(text);
                var joinedText = string.Join(" ", words);
                var encodedData = Uri.EscapeDataString(joinedText);

                var wordCloudUrl = $"https://quickchart.io/wordcloud?text={encodedData}";

                _logger.LogInformation("Generated word cloud URL");

                return wordCloudUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating word cloud URL");
                return string.Empty;
            }
        }

        public async Task<string> GetOrGenerateWordCloudUrlAsync(string fileId, string content)
        {
            try
            {
                if (!Guid.TryParse(fileId, out var parsedFileId))
                {
                    throw new ArgumentException("Invalid file ID format", nameof(fileId));
                }
                
                // Check if analysis already exists with a word cloud URL
                var existingAnalysis = await _dbContext.Analyses
                    .Where(a => a.FileId == parsedFileId && a.WordCloudUrl != null)
                    .FirstOrDefaultAsync();
                
                if (existingAnalysis != null && !string.IsNullOrEmpty(existingAnalysis.WordCloudUrl))
                {
                    _logger.LogInformation("Word cloud URL already exists for file: {FileId}", fileId);
                    return existingAnalysis.WordCloudUrl;
                }
                
                // Generate new word cloud URL
                var wordCloudUrl = GenerateWordCloudUrl(content);
                
                if (existingAnalysis != null)
                {
                    existingAnalysis.WordCloudUrl = wordCloudUrl;
                    await _dbContext.SaveChangesAsync();
                }
                
                return wordCloudUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or generating word cloud URL for file: {FileId}", fileId);
                return string.Empty;
            }
        }

        private List<string> ExtractWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^\w\s]", " ");

            var words = Regex.Split(text, @"\s+")
                .Where(w => !string.IsNullOrWhiteSpace(w) && w.Length > 3)
                .ToList();

            return words;
        }
    }
}
