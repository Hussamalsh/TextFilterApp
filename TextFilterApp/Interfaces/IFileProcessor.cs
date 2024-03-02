namespace TextFilterApp.Interfaces;

public interface IFileProcessor
{
    Task ProcessFileAsync(string fileName, CancellationToken cancellationToken = default);
}