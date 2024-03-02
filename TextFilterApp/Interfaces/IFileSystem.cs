namespace TextFilterApp.Interfaces;

public interface IFileSystem
{
    bool FileExists(string path);
    IAsyncEnumerable<IEnumerable<string>> ReadFileInChunksAsync(string filePath, int chunkSize, CancellationToken cancellationToken = default);
}

