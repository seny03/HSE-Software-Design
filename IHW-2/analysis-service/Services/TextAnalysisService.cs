using AnalysisService.Data;
using AnalysisService.Models;
using F23.StringSimilarity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AnalysisService.Services
{
    public class TextAnalysisService : ITextAnalysisService
    {
        private readonly AnalysisDbContext _dbContext;
        private readonly IWordCloudService _wordCloudService;
        private readonly ILogger<TextAnalysisService> _logger;
        private readonly NormalizedLevenshtein _levenshtein;

        public TextAnalysisService(
            AnalysisDbContext dbContext,
            IWordCloudService wordCloudService,
            ILogger<TextAnalysisService> logger)
        {
            _dbContext = dbContext;
            _wordCloudService = wordCloudService;
            _logger = logger;
            _levenshtein = new NormalizedLevenshtein();
        }

        public async Task<AnalysisResult> AnalyzeTextAsync(string fileId, string content)
        {
            try
            {
                _logger.LogInformation("Analyzing file: {FileId}", fileId);
                
                if (string.IsNullOrEmpty(content))
                {
                    throw new ArgumentException("Content cannot be empty", nameof(content));
                }
                
                if (!Guid.TryParse(fileId, out var parsedFileId))
                {
                    throw new ArgumentException("Invalid file ID format", nameof(fileId));
                }
                
                // Check if analysis already exists
                var existingAnalysis = await _dbContext.Analyses
                    .Where(a => a.FileId == parsedFileId)
                    .FirstOrDefaultAsync();
                
                if (existingAnalysis != null)
                {
                    _logger.LogInformation("Analysis already exists for file: {FileId}", fileId);
                    
                    return new AnalysisResult
                    {
                        AnalysisId = existingAnalysis.Id.ToString(),
                        FileId = fileId,
                        ParagraphCount = existingAnalysis.ParagraphCount,
                        WordCount = existingAnalysis.WordCount,
                        CharacterCount = existingAnalysis.CharacterCount,
                        WordCloudUrl = existingAnalysis.WordCloudUrl,
                        CreatedAt = existingAnalysis.CreatedAt
                    };
                }
                
                // Perform analysis
                var stats = CountStatistics(content);
                
                // Generate word cloud URL
                var wordCloudUrl = await _wordCloudService.GetOrGenerateWordCloudUrlAsync(fileId, content);
                
                // Create and save analysis entity
                var analysisEntity = new AnalysisEntity
                {
                    Id = Guid.NewGuid(),
                    FileId = parsedFileId,
                    ParagraphCount = stats.ParagraphCount,
                    WordCount = stats.WordCount,
                    CharacterCount = stats.CharacterCount,
                    WordCloudUrl = wordCloudUrl,
                    CreatedAt = DateTime.UtcNow
                };
                
                _dbContext.Analyses.Add(analysisEntity);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Analysis completed for file: {FileId}, ID: {AnalysisId}", 
                    fileId, analysisEntity.Id);
                
                return new AnalysisResult
                {
                    AnalysisId = analysisEntity.Id.ToString(),
                    FileId = fileId,
                    ParagraphCount = stats.ParagraphCount,
                    WordCount = stats.WordCount,
                    CharacterCount = stats.CharacterCount,
                    WordCloudUrl = wordCloudUrl,
                    CreatedAt = analysisEntity.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing file: {FileId}", fileId);
                throw;
            }
        }

        public async Task<AnalysisResult?> GetAnalysisAsync(Guid analysisId)
        {
            try
            {
                var analysis = await _dbContext.Analyses.FindAsync(analysisId);
                
                if (analysis == null)
                {
                    _logger.LogWarning("Analysis not found: {AnalysisId}", analysisId);
                    return null;
                }
                
                return new AnalysisResult
                {
                    AnalysisId = analysis.Id.ToString(),
                    FileId = analysis.FileId.ToString(),
                    ParagraphCount = analysis.ParagraphCount,
                    WordCount = analysis.WordCount,
                    CharacterCount = analysis.CharacterCount,
                    WordCloudUrl = analysis.WordCloudUrl,
                    CreatedAt = analysis.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis: {AnalysisId}", analysisId);
                throw;
            }
        }

        public async Task<List<ComparisonResult>> CompareTextsAsync(List<string> fileIds, Dictionary<string, string> fileContents)
        {
            try
            {
                if (fileIds.Count < 2)
                {
                    throw new ArgumentException("At least two files are required for comparison", nameof(fileIds));
                }
                
                var results = new List<ComparisonResult>();
                
                // Compare each pair of files
                for (int i = 0; i < fileIds.Count; i++)
                {
                    for (int j = i + 1; j < fileIds.Count; j++)
                    {
                        var fileId1 = fileIds[i];
                        var fileId2 = fileIds[j];
                        
                        if (!fileContents.TryGetValue(fileId1, out var content1) || 
                            !fileContents.TryGetValue(fileId2, out var content2))
                        {
                            throw new KeyNotFoundException("One or more file contents not found");
                        }
                        
                        // Calculate similarity
                        var similarityScore = CalculateSimilarity(content1, content2);
                        
                        // Create and save comparison entity
                        var comparisonEntity = new ComparisonEntity
                        {
                            Id = Guid.NewGuid(),
                            FileId1 = Guid.Parse(fileId1),
                            FileId2 = Guid.Parse(fileId2),
                            SimilarityScore = similarityScore,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _dbContext.Comparisons.Add(comparisonEntity);
                        
                        // Add to results
                        results.Add(new ComparisonResult
                        {
                            ComparisonId = comparisonEntity.Id.ToString(),
                            FileId1 = fileId1,
                            FileId2 = fileId2,
                            SimilarityScore = similarityScore,
                            IsPlagiarism = similarityScore > 0.8 // Threshold for plagiarism detection
                        });
                    }
                }
                
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Compared {Count} files, found {ResultCount} comparisons", 
                    fileIds.Count, results.Count);
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing files");
                throw;
            }
        }

        public double CalculateSimilarity(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return 0;
            }
            
            // Normalize texts
            text1 = NormalizeText(text1);
            text2 = NormalizeText(text2);
            
            // Return 1 - normalized Levenshtein distance
            return 1 - _levenshtein.Distance(text1, text2);
        }

        public AnalysisStatistics CountStatistics(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new AnalysisStatistics();
            }
            
            // Count paragraphs (separated by double newlines)
            var paragraphs = Regex.Split(text, @"\n\s*\n")
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();
            
            // Count words
            var words = Regex.Split(text, @"\s+")
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList();
            
            // Count characters (excluding whitespace)
            var characters = text.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Length;
            
            return new AnalysisStatistics
            {
                ParagraphCount = paragraphs.Count,
                WordCount = words.Count,
                CharacterCount = characters
            };
        }

        private string NormalizeText(string text)
        {
            // Remove special characters and extra whitespace
            return Regex.Replace(text.ToLower(), @"[^\w\s]", "")
                .Trim()
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
        }
    }
}
