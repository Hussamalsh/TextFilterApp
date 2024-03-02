using TextFilterApp.Interfaces;

namespace TextFilterApp.Filters;

public class VowelMiddleFilter : IFilterStrategy
{
    private readonly char[] Vowels = { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

    public string Apply(string input)
    {
        return string.Join(" ", input.Split().Where(word => !HasVowelInMiddle(word)));
    }

    private bool HasVowelInMiddle(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;

        int middleIndex = word.Length / 2;
        if (word.Length % 2 == 0)
        {
            // For even-length words, check both middle characters for a vowel
            return Vowels.Contains(word[middleIndex - 1]) || Vowels.Contains(word[middleIndex]);
        }
        else
        {
            // For odd-length words, check the middle character for a vowel
            return Vowels.Contains(word[middleIndex]);
        }
    }
}
