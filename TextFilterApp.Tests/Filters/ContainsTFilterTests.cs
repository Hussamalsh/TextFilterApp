using TextFilterApp.Filters;

namespace TextFilterApp.Tests.Filters;

[TestFixture]
public class ContainsTFilterTests
{
    private ContainsTFilter _filter;

    [SetUp]
    public void Setup()
    {
        _filter = new ContainsTFilter();
    }

    [Test]
    public void Apply_WhenInputDoesNotContainT_ReturnsOriginalInput()
    {
        var input = "hello world";
        var result = _filter.Apply(input);
        Assert.AreEqual(input, result, "Expected input to remain unchanged when it does not contain 't'.");
    }

    [Test]
    public void Apply_WhenAllWordsContainT_ReturnsEmptyString()
    {
        var input = "test text totally";
        var expected = "";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected an empty string when all words contain 't'.");
    }

    [Test]
    public void Apply_WhenInputIsMixedCase_RemovesWordsWithTRegardlessOfCase()
    {
        var input = "Test input with Text";
        var expected = "";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected all words with 't' to be removed, regardless of case.");
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
    public void Apply_WhenSomeWordsContainT_RemovesOnlyThoseWords()
    {
        var input = "keep this test but not that";
        var expected = "keep";
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected only words containing 't' to be removed, with normalized spaces.");
    }

    [Test]
    public void Apply_WithWhitespaceHandling_IgnoresExcessWhitespace()
    {
        var input = "  test   input  with    text  ";
        var expected = ""; 
        var result = _filter.Apply(input); 
        Assert.AreEqual(expected, result, "Expected function to ignore excess whitespace and return an empty string.");
    }

    [Test]
    public void Apply_WhenInputContainsPunctuation_PunctuationPreserved()
    {
        var input = "This, indeed, is a test.";
        var expected = ", indeed, is a ."; 
        var result = _filter.Apply(input);
        Assert.AreEqual(expected, result, "Expected punctuation to be preserved and words with 't' removed, with spaces normalized.");
    }
}
