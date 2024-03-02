using System.Collections.Concurrent;
using System.Text;
using TextFilterApp.Configuration;
using TextFilterApp.Interfaces;

namespace TextFilterApp;

public class TextProcessor : IFileProcessor
{
    private readonly ITextFilter _textFilter;
    private readonly AppSettings _settings;
    private readonly IFileSystem _fileSystem;

    public TextProcessor(ITextFilter textFilter, AppSettings settings, IFileSystem fileSystem)
    {
        _textFilter = textFilter;
        _settings = settings;
        _fileSystem = fileSystem;
    }

    public async Task ProcessFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!_fileSystem.FileExists(filePath))
        {
            Console.WriteLine("File does not exist.");
            return;
        }

        var chunkIndices = new ConcurrentDictionary<int, string>();
        int index = 0;
        bool hasContent = false;

        try
        {
            await foreach (var linesChunk in _fileSystem.ReadFileInChunksAsync(filePath, _settings.ChunkSize, cancellationToken))
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                hasContent = true; // Mark that we have encountered content
                var currentIndex = index++; // Capture the current index for ordering
                var textChunk = string.Join(Environment.NewLine, linesChunk);
                var filteredChunk = await Task.Run(() => _textFilter.ApplyFilters(textChunk));
                chunkIndices[currentIndex] = filteredChunk;
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
            return;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An I/O error occurred while reading the file: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while reading the file: {ex.Message}");
            return;
        }

        if (hasContent)
        {
            var stringBuilder = new StringBuilder();
            foreach (var chunk in chunkIndices.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
            {
                stringBuilder.AppendLine(chunk);
            }
            Console.Write(stringBuilder.ToString()); 
        }
    }
}
