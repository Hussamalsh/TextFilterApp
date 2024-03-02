using System.Text.RegularExpressions;
using TextFilterApp.Interfaces;

namespace TextFilterApp.Filters;

public class MinLengthFilter : IFilterStrategy
{
    public string Apply(string input)
    {
        if (input == null) return null;

        // Split the input into tokens while preserving words and punctuation as separate entities.
        var tokens = Regex.Matches(input, @"(\w+|\s|[^\w\s])")
                          .Cast<Match>()
                          .Select(m => m.Value);

        // Reconstruct the string, applying the minimum length filter to words only, and managing spaces correctly.
        var result = new System.Text.StringBuilder();
        foreach (var token in tokens)
        {
            if (Regex.IsMatch(token, @"^\w+$")) // Token is a word.
            {
                if (token.Length >= 3)
                {
                    // Append word if it meets the length requirement.
                    if (result.Length > 0 && result[result.Length - 1] != ' ') result.Append(" ");
                    result.Append(token);
                }
            }
            else if (Regex.IsMatch(token, @"[^\w\s]")) // Token is punctuation.
            {
                result.Append(token);
            }
            else if (token == " " && result.Length > 0 && result[result.Length - 1] != ' ' && result[result.Length - 1] != ',')
            {
                // Append space if it's not leading, not after a comma, and the last character wasn't already a space.
                result.Append(token);
            }
        }

        return result.ToString().Trim();
    }
}
