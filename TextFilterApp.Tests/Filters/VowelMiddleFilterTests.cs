using TextFilterApp.Filters;

namespace TextFilterApp.Tests.Filters;

[TestFixture]
public class VowelMiddleFilterTests
{
    private VowelMiddleFilter _filter;

    [SetUp]
    public void Setup()
    {
        _filter = new VowelMiddleFilter();
    }


    // Test Case 1: Empty String Input
    [TestCase("", ExpectedResult = "")]
    public string Apply_GivenEmptyString_ReturnsEmpty(string input)
    {
        return _filter.Apply(input);
    }

    // Test Case 2: Single Word Without Vowel in the Middle
    [TestCase("text", ExpectedResult = "")]
    [TestCase("crypt", ExpectedResult = "crypt")]
    public string Apply_SingleWordWithoutVowelMiddle_ReturnsUnchanged(string input)
    {
        return _filter.Apply(input);
    }

    // Test Case 3: Single Word With Vowel in the Middle
    [TestCase("cat", ExpectedResult = "")]
    [TestCase("dog", ExpectedResult = "")]
    public string Apply_SingleWordWithVowelMiddle_IsFilteredOut(string input)
    {
        return _filter.Apply(input);
    }

    // Test Case 4: Multiple Words, Mixed Conditions
    [TestCase("cat dog text Laptop", ExpectedResult = "Laptop")]
    [TestCase("read roof cat", ExpectedResult = "")]
    public string Apply_MultipleWordsMixedConditions_CorrectlyFilters(string input)
    {
        return _filter.Apply(input);
    }

    // Test Case 5: Words with Even Length
    [TestCase("read", ExpectedResult = "")]
    [TestCase("roof", ExpectedResult = "")]
    public string Apply_EvenLengthWords_CorrectBehavior(string input)
    {
        return _filter.Apply(input);
    }

    // Test Case 6: Words with Odd Length
    [TestCase("bat", ExpectedResult = "")]
    [TestCase("beet", ExpectedResult = "")] 
    public string Apply_OddLengthWords_CorrectBehavior(string input)
    {
        return _filter.Apply(input);
    }


    // Test Case 7: Case Sensitivity
    [TestCase("Read", ExpectedResult = "")]
    [TestCase("Roof", ExpectedResult = "")]
    public string Apply_CaseSensitivity_CorrectlyIdentifiesVowels(string input)
    {
        return _filter.Apply(input);
    }

    [TestCase("clean", ExpectedResult = "")] // "clean" is filtered because the middle is 'e'
    [TestCase("what", ExpectedResult = "")] // "what" is filtered because the middle is 'ha'
    [TestCase("currently", ExpectedResult = "")] // "currently" is filtered because the middle is 'e'
    [TestCase("the", ExpectedResult = "the")] // "the" is not filtered; no vowel in the exact middle
    [TestCase("rather", ExpectedResult = "rather")] // "rather" is not filtered; 'th' in the middle, no vowel
    public string Apply_FilterWordsWithVowelInMiddle_CorrectBehavior(string input)
    {
        return _filter.Apply(input);
    }

}
