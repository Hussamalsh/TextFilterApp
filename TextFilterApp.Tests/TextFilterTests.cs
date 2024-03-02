using TextFilterApp.Interfaces;

namespace TextFilterApp.Tests;

[TestFixture]
public class TextFilterTests
{
    private List<IFilterStrategy> filters;
    private TextFilter textFilter;

    [SetUp]
    public void Setup()
    {
        // Setup can be adjusted per test case if necessary
        filters = new List<IFilterStrategy>();
        textFilter = new TextFilter(filters);
    }

    public class AppendFilter : IFilterStrategy
    {
        private readonly string _suffix;

        public AppendFilter(string suffix)
        {
            _suffix = suffix;
        }

        public string Apply(string input) => input + _suffix;
    }

    public class UppercaseFilter : IFilterStrategy
    {
        public string Apply(string input)
        {
            // Check for null input to avoid NullReferenceException
            if (input == null)
            {
                return null;
            }

            return input.ToUpper();
        }
    }


    [Test]
    public void ApplyFilters_WhenFiltersAreApplied_ModifiesInputAccordingly()
    {
        // Arrange
        filters.Add(new AppendFilter("A"));
        filters.Add(new AppendFilter("B"));

        // Act
        var result = textFilter.ApplyFilters("Test");

        // Assert
        Assert.AreEqual("TestAB", result);
    }

    [Test]
    public void ApplyFilters_WhenNoFiltersProvided_ReturnsOriginalInput()
    {
        // Act
        var result = textFilter.ApplyFilters("Test");

        // Assert
        Assert.AreEqual("Test", result);
    }

    [Test]
    public void ApplyFilters_WhenInputIsEmpty_ReturnsEmptyString()
    {
        // Act
        var result = textFilter.ApplyFilters(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [Test]
    public void ApplyFilters_WhenInputIsNull_ReturnsNull()
    {
        // Arrange
        filters.Add(new UppercaseFilter());

        string input = null;

        // Act
        var result = textFilter.ApplyFilters(input);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void ApplyFilters_WhenFiltersTransformContent_TransformsAsExpected()
    {
        // Arrange
        filters.Add(new UppercaseFilter());

        // Act
        var result = textFilter.ApplyFilters("Test");

        // Assert
        Assert.AreEqual("TEST", result);
    }

    [Test]
    public void ApplyFilters_WhenMultipleFiltersAdded_AppliesInProvidedOrder()
    {
        // Arrange
        filters.Add(new AppendFilter("A"));
        filters.Add(new UppercaseFilter()); // This should uppercase the entire string including 'A'

        // Act
        var result = textFilter.ApplyFilters("Test");

        // Assert
        Assert.AreEqual("TESTA", result);
    }
}
