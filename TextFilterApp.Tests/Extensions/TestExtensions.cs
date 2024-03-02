using System.Runtime.CompilerServices;


namespace TextFilterApp.Tests.Extensions;

public static class TestExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return await Task.FromResult(item);
        }
    }
}