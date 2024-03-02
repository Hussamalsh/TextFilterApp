using System.Threading;
using TextFilterApp.Interfaces;

namespace TextFilterApp;

public class FileSystem : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);

    public async IAsyncEnumerable<IEnumerable<string>> ReadFileInChunksAsync(string filePath, int chunkSize, CancellationToken cancellationToken = default)
    {
        // Validate chunkSize before proceeding
        if (chunkSize <= 0)
        {
            throw new ArgumentException("Chunk size must be greater than 0.", nameof(chunkSize));
        }

        // Ensure the file exists to avoid unnecessary work
        if (!FileExists(filePath))
        {
            yield break;
        }

        // Open the file with read access
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var streamReader = new StreamReader(fileStream))
        {
            var lines = new List<string>(chunkSize);
            string line;

            // Read lines asynchronously from the file until no lines remain
            while ((line = await streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                lines.Add(line);
                if (lines.Count == chunkSize)
                {
                    yield return lines.ToArray(); // Return a copy of the list to avoid reference issues
                    lines.Clear();
                }
            }

            // If there are remaining lines after the loop, return them as a final chunk
            if (lines.Count > 0)
            {
                yield return lines.ToArray(); // Ensure the last chunk is also returned
            }
        }
    }
}
