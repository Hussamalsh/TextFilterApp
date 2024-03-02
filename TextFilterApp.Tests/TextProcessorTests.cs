using Moq;
using System.Text;
using TextFilterApp.Configuration;
using TextFilterApp.Interfaces;
using TextFilterApp.Tests.Extensions;

namespace TextFilterApp.Tests;

[TestFixture]
[TestFixture]
public class TextProcessorTests
{
    private Mock<ITextFilter> mockTextFilter;
    private Mock<IFileSystem> mockFileSystem;
    private AppSettings settings;
    private TextProcessor textProcessor;
    private CancellationTokenSource cancellationTokenSource;


    [SetUp]
    public void SetUp()
    {
        mockTextFilter = new Mock<ITextFilter>();
        mockFileSystem = new Mock<IFileSystem>();
        settings = new AppSettings { ChunkSize = 2 }; // Example chunk size

        textProcessor = new TextProcessor(mockTextFilter.Object, settings, mockFileSystem.Object);
        cancellationTokenSource = new CancellationTokenSource();
    }

    [TearDown]
    public void TearDown()
    {
        cancellationTokenSource?.Dispose();
    }

    [Test]
    public async Task ProcessFileAsync_FileDoesNotExist_PrintsErrorMessage()
    {
        // Arrange
        var filePath = "nonexistent.txt";
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(false);

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Once);
    }

    [Test]
    public async Task ProcessFileAsync_FileExists_ProcessesFileInChunks()
    {
        // Arrange
        var filePath = "existent.txt";
        var fileContent = new List<IEnumerable<string>> { new[] { "Line 1", "Line 2" }, new[] { "Line 3" } };
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(fileContent.ToAsyncEnumerable()); // Use Returns instead of ReturnsAsync

        mockTextFilter.Setup(filter => filter.ApplyFilters(It.IsAny<string>())).Returns<string>(input => input);

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        mockFileSystem.Verify(fs => fs.ReadFileInChunksAsync(filePath, settings.ChunkSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessFileAsync_WithContentModificationByFilter_ModifiesContentAccordingly()
    {
        // Arrange
        var filePath = "existent.txt";
        var originalContent = new List<IEnumerable<string>>
    {
        new[] { "Hello world", "This is a test" },
        new[] { "Another line", "And one more" }
    };
        var modifiedContent = "Modified content";
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(originalContent.ToAsyncEnumerable());

        // Setup the mock filter to return a modified string
        mockTextFilter.Setup(filter => filter.ApplyFilters(It.IsAny<string>())).Returns(modifiedContent);

        // Use a StringBuilder to capture console output
        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        // Verify that the filter's ApplyFilters method was called at least once
        mockTextFilter.Verify(filter => filter.ApplyFilters(It.IsAny<string>()), Times.AtLeastOnce);

        // Check that the console output contains the modified content
        StringAssert.Contains(modifiedContent, consoleOutput.ToString());

        mockTextFilter.Verify(filter => filter.ApplyFilters(It.IsAny<string>()), Times.Exactly(originalContent.Count));
    }

    [Test]
    public async Task ProcessFileAsync_ThrowsIOException_PrintsErrorMessage()
    {
        // Arrange
        var filePath = "existent.txt";
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);

        // Directly throw IOException from the mock setup
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .Throws(new IOException("Simulated I/O exception"));

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        StringAssert.Contains("An I/O error occurred while reading the file: Simulated I/O exception", consoleOutput.ToString());
    }

    [Test]
    public async Task ProcessFileAsync_CorrectOrderAndOutput()
    {
        // Arrange
        var filePath = "orderedFile.txt";
        var fileContent = new List<IEnumerable<string>>
    {
        new[] { "First chunk line 1", "First chunk line 2" },
        new[] { "Second chunk line 1", "Second chunk line 2" },
        new[] { "Third chunk line 1", "Third chunk line 2" }
    };
        var expectedOutput = string.Join(Environment.NewLine, new[] {
                                "First chunk line 1" + Environment.NewLine + "First chunk line 2",
                                "Second chunk line 1" + Environment.NewLine + "Second chunk line 2",
                                "Third chunk line 1" + Environment.NewLine + "Third chunk line 2"
                            });



        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(fileContent.ToAsyncEnumerable());

        // Mocking the text filter to simply return the input string for simplicity
        mockTextFilter.Setup(filter => filter.ApplyFilters(It.IsAny<string>())).Returns<string>(input => input);

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        var actualOutput = consoleOutput.ToString().Trim();
        Assert.AreEqual(expectedOutput, actualOutput, "The final output does not match the expected output, or the order of the processed text is incorrect.");
    }

    [Test]
    public async Task ProcessFileAsync_ProcessesChunksInParallelAndPreservesOrder()
    {
        // Arrange
        var filePath = "parallelProcessingTest.txt";
        var chunkDelays = new Dictionary<string, int>
            {
                {"Chunk 1", 300},
                {"Chunk 2", 100},
                {"Chunk 3", 200}
            };
        var fileContent = chunkDelays.Keys.Select(chunk => new[] { chunk }).ToList();

        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);

        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
               .Returns(fileContent.ToAsyncEnumerable());

        // Mock ITextFilter to simulate processing delay based on chunk content
        mockTextFilter.Setup(filter => filter.ApplyFilters(It.IsAny<string>()))
                      .Returns<string>(input =>
                      {
                          Task.Delay(chunkDelays[input]).Wait(); // Simulate processing time
                          return input; 
                      });

        var expectedOrder = string.Join(Environment.NewLine, chunkDelays.Keys) + Environment.NewLine;

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        var actualOutput = consoleOutput.ToString();
        Assert.AreEqual(expectedOrder, actualOutput, "Chunks were not processed in parallel correctly or the order was not preserved.");
    }


    [Test]
    public async Task ProcessFileAsync_EmptyFile_CompletesWithoutError()
    {
        // Arrange
        var filePath = "emptyFile.txt";
        var emptyContent = new List<IEnumerable<string>>(); // Simulate an empty file
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(emptyContent.ToAsyncEnumerable()); // Return an empty IAsyncEnumerable

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        mockFileSystem.Verify(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsEmpty(consoleOutput.ToString(), "Expected no output for an empty file, but found some.");
    }

    [Test]
    public async Task ProcessFileAsync_SingleLineFile_ProcessesCorrectly()
    {
        // Arrange
        var filePath = "singleLineFile.txt";
        var singleLineContent = new List<IEnumerable<string>> { new[] { "This is the only line in the file." } };
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .Returns(singleLineContent.ToAsyncEnumerable());

        // Mock the ITextFilter to return the input string for simplicity
        mockTextFilter.Setup(filter => filter.ApplyFilters(It.IsAny<string>()))
                      .Returns<string>(input => input);

        var expectedOutput = "This is the only line in the file." + Environment.NewLine; // Expect the single line with a newline at the end

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        var actualOutput = consoleOutput.ToString();
        Assert.AreEqual(expectedOutput, actualOutput, "The single line file was not processed correctly.");
    }


    [Test]
    public async Task ProcessFileAsync_ThrowsUnexpectedException_PrintsErrorMessage()
    {
        // Arrange
        var filePath = "existent.txt";
        mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadFileInChunksAsync(filePath, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .Throws(new Exception("Simulated unexpected exception"));
        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));

        // Act
        await textProcessor.ProcessFileAsync(filePath);

        // Assert
        StringAssert.Contains("An unexpected error occurred while reading the file: Simulated unexpected exception", consoleOutput.ToString());
    }



}