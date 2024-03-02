using TextFilterApp.Filters;
using TextFilterApp.Interfaces;

namespace TextFilterApp.Tests.Filters;

[TestFixture]
public class MinLengthFilterTests
{
    private IFilterStrategy _filter;

    [SetUp]
    public void Setup()
    {
        _filter = new MinLengthFilter();
    }

    [Test]
    public void Apply_RemovesShortWords()
    {
        var input = "An apple a day keeps the doctor away";
        var expected = "apple day keeps the doctor away";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Short words should be removed.");
    }

    [Test]
    public void Apply_WhenInputIsEmpty_ReturnsEmptyString()
    {
        var input = "";
        var result = _filter.Apply(input);
        Assert.AreEqual(input, result, "Expected an empty string input to return an empty string.");
    }

    [Test]
    public void Apply_WhenInputIsNull_ReturnsNull()
    {
        string input = null;
        var result = _filter.Apply(input);
        Assert.IsNull(result, "Expected null input to return null.");
    }

    [Test]
    public void Apply_WithMixedCase_RemainsUnchanged()
    {
        var input = "Big apple Tiny egg";
        var expected = "Big apple Tiny egg";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected mixed-case words meeting length criteria to remain unchanged.");
    }

    [Test]
    public void Apply_WithPunctuation_PunctuationConsideredPartOfWord()
    {
        var input = "Wow! Really? Yes.";
        var expected = "Wow! Really? Yes."; 
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected words with punctuation to be considered in length calculation.");
    }

    [Test]
    public void Apply_WithWhitespaceHandling_IgnoresExcessWhitespace()
    {
        var input = "  yes   no ok    ";
        var expected = "yes"; 
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected function to normalize spaces and filter short words.");
    }

    [Test]
    public void Apply_WhenAllWordsAreShort_ReturnsEmptyString()
    {
        var input = "it is a";
        var expected = ""; 
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected an empty string when all words are short.");
    }

    [Test]
    public void Apply_WhenInputContainsPunctuation_PunctuationPreserved()
    {
        var input = "No, indeed, is a test.";
        var expected = ", indeed, test.";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected punctuation to be preserved.");
    }
}
