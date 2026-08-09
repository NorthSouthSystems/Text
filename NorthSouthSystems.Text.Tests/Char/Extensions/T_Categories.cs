public class T_CharExtensions_Categories
{
    [Theory]
    [InlineData('a')]
    [InlineData('A')]
    [InlineData('1')]
    [InlineData('.')]
    [InlineData(' ')]
    [InlineData('*')]
    [InlineData('@')]
    [InlineData('\t')]
    public void IsInAnyCategoryNoneAll(char c)
    {
        c.IsInAnyCategory(CharCategories.None).Should().BeFalse();
        c.IsInAnyCategory(CharCategories.All).Should().BeTrue();
    }

    [Theory]
    [InlineData('a', new[] { CharCategories.Letter, CharCategories.Lower })]
    [InlineData('A', new[] { CharCategories.Letter, CharCategories.Upper })]
    [InlineData('1', new[] { CharCategories.Digit, CharCategories.Number })]
    [InlineData('.', new[] { CharCategories.Punctuation })]
    [InlineData('!', new[] { CharCategories.Punctuation })]
    [InlineData(' ', new[] { CharCategories.Separator, CharCategories.WhiteSpace })]
    [InlineData('\t', new[] { CharCategories.Control, CharCategories.WhiteSpace })]
    [InlineData('+', new[] { CharCategories.Symbol })]
    [InlineData('<', new[] { CharCategories.Symbol })]
    public void IsInAnyCategoryVarious(char c, CharCategories[] categories)
    {
        foreach (var category in categories)
            c.IsInAnyCategory(category).Should().BeTrue();

        c.IsInAnyCategory(
                categories.Aggregate(CharCategories.None, (accumulate, category) => accumulate | category))
            .Should()
            .BeTrue();

        c.IsInAnyCategory(
                categories.Aggregate(CharCategories.All, (accumulate, category) => accumulate ^ category))
            .Should()
            .BeFalse();
    }
}
