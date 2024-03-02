using System.Text.RegularExpressions;
using TextFilterApp.Interfaces;

namespace TextFilterApp.Filters;

public class ContainsTFilter : IFilterStrategy
{
    public string Apply(string input)
    {
        if (input == null) return null;

        var tokens = Regex.Matches(input, @"(\b\w+\b)|(\W+)")
                          .Cast<Match>()
                          .Select(m => m.Value);

        var filteredTokens = tokens.Where(token =>
            !Regex.IsMatch(token, @"\b\w*t\w*\b", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(token, @"^\W+$"));

        // Concatenate filtered tokens and collapse multiple spaces into a single space
        var result = string.Join("", filteredTokens);
        result = Regex.Replace(result, @"\s+", " ").Trim(); // Collapses multiple spaces and trims the result

        return result;
    }
}
