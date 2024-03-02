

namespace TextFilterApp.Tests;

[TestFixture]
public class FileSystemTests
{
    private FileSystem _fileSystem;
    private string _existingFilePath = "textinput.txt"; // Update with actual path
    private string _nonExistingFilePath = "path/to/nonexistingfile.txt";

    [SetUp]
    public void Setup()
    {
        _fileSystem = new FileSystem();
    }

    [Test]
    public void FileExists_ExistingFile_ReturnsTrue()
    {
        var result = _fileSystem.FileExists(_existingFilePath);
        Assert.IsTrue(result);
    }

    [Test]
    public void FileExists_NonExistingFile_ReturnsFalse()
    {
        var result = _fileSystem.FileExists(_nonExistingFilePath);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task ReadFileInChunksAsync_ExistingFile_ReturnsAllLines()
    {
        int chunkSize = 10; // Adjust based on your test file
        var linesRead = new List<string>();

        await foreach (var chunk in _fileSystem.ReadFileInChunksAsync(_existingFilePath, chunkSize))
        {
            linesRead.AddRange(chunk);
        }

        // Assert that linesRead contains all lines from your file
        Assert.IsNotEmpty(linesRead);
    }

    [Test]
    public async Task ReadFileInChunksAsync_NonExistingFile_ReturnsEmptyCollection()
    {
        int chunkSize = 10;
        bool hasData = false;

        await foreach (var chunk in _fileSystem.ReadFileInChunksAsync(_nonExistingFilePath, chunkSize))
        {
            if (chunk.Any()) hasData = true;
        }

        Assert.IsFalse(hasData);
    }

    [Test]
    public void ReadFileInChunksAsync_Cancellation_ThrowsOperationCanceledException()
    {
        int chunkSize = 10;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () => 
        {
            await foreach (var _ in _fileSystem.ReadFileInChunksAsync(_existingFilePath, chunkSize, cts.Token)) { }
        });
    }

    [Test]
    public void ReadFileInChunksAsync_ZeroChunkSize_ThrowsArgumentException()
    {
        int chunkSize = 0;
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => {
            await foreach (var _ in _fileSystem.ReadFileInChunksAsync(_existingFilePath, chunkSize)) { }
        });

        Assert.IsTrue(ex.Message.Contains("Chunk size must be greater than 0."));
    }

    [Test]
    public async Task ReadFileInChunksAsync_ChunkSizeLargerThanFile_ReturnsAllLinesInSingleChunk()
    {
        int chunkSize = 30; // Larger than the total number of lines in the file
        List<IEnumerable<string>> chunksCollected = new List<IEnumerable<string>>();

        await foreach (var chunk in _fileSystem.ReadFileInChunksAsync(_existingFilePath, chunkSize))
        {
            chunksCollected.Add(chunk);
        }

        // Assert that only one chunk is returned, containing all the lines of the file
        Assert.AreEqual(1, chunksCollected.Count, "Expected only a single chunk to be returned.");
        Assert.AreEqual(21, chunksCollected.First().Count(), "The single chunk should contain all the lines from the file.");
    }




}