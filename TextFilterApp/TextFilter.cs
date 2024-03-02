using TextFilterApp.Interfaces;

namespace TextFilterApp;

public class TextFilter : ITextFilter
{
    private readonly IEnumerable<IFilterStrategy> _filters;

    public TextFilter(IEnumerable<IFilterStrategy> filters)
    {
        _filters = filters;
    }

    public string ApplyFilters(string input)
    {
        foreach (var filter in _filters)
        {
            input = filter.Apply(input);
        }
        return input;
    }
}